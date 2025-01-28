package com.rhodry.connect.transforms;

import org.apache.kafka.connect.connector.ConnectRecord;
import org.apache.kafka.connect.data.Field;
import org.apache.kafka.connect.data.Schema;
import org.apache.kafka.connect.data.SchemaBuilder;
import org.apache.kafka.connect.data.Struct;
import org.apache.kafka.connect.errors.DataException;
import org.apache.kafka.connect.transforms.Transformation;
import org.apache.kafka.connect.transforms.util.SchemaUtil;
import org.apache.kafka.connect.transforms.util.SimpleConfig;

import org.apache.kafka.common.cache.Cache;
import org.apache.kafka.common.cache.LRUCache;
import org.apache.kafka.common.cache.SynchronizedCache;
import org.apache.kafka.common.config.ConfigDef;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class CDCLog<R extends ConnectRecord<R>> implements Transformation<R> {
    
    private static final Logger LOGGER = LoggerFactory.getLogger(CDCLog.class);

    public static final String OVERVIEW_DOC = "For transforming Debezium"
        + "MySQL source connector envelopes to warehouse CDC log formats.";

    static String OP_FIELD_NAME = "_operation";
    static String SOURCE_TS_FIELD_NAME = "_source_timestamp_utc";
    static String QUEUED_TS_FIELD_NAME = "_queued_timestamp_utc";
    static String UPDATE_MASK_FIELD_NAME = "_update_mask";

    static final String OP_FIELD_CONFIG_NAME = "field.op.name";
    static final String QUEUED_TS_FIELD_CONFIG_NAME = "field.ts_ms.name";
    static final String SOURCE_TS_FIELD_CONFIG_NAME = "field.source.ts_ms.name";
    static final String UPDATE_MASK_FIELD_CONFIG_NAME = "field.update_mask.name";

    private static final ConfigDef CONFIG_DEF = new ConfigDef()
        .define(
            OP_FIELD_CONFIG_NAME,
            ConfigDef.Type.STRING,
            OP_FIELD_NAME,
            ConfigDef.Importance.LOW,
            "Name of the `op` field.")
        .define(
            SOURCE_TS_FIELD_CONFIG_NAME,
            ConfigDef.Type.STRING,
            SOURCE_TS_FIELD_NAME,
            ConfigDef.Importance.LOW,
            "Name of the `source.ts_ms` field.")
        .define(
            QUEUED_TS_FIELD_CONFIG_NAME,
            ConfigDef.Type.STRING,
            QUEUED_TS_FIELD_NAME,
            ConfigDef.Importance.LOW,
            "Name of the `ts_ms` field.")
        .define(
            UPDATE_MASK_FIELD_CONFIG_NAME,
            ConfigDef.Type.STRING,
            UPDATE_MASK_FIELD_NAME,
            ConfigDef.Importance.LOW,
            "Name of the `update_mask` field.");

    private static final String PURPOSE = "cdc-log";

    private Cache<Schema, Schema> cdcSchemaCache;

    @Override
    public void configure(Map<String, ?> configs) {
        final SimpleConfig config = new SimpleConfig(CONFIG_DEF, configs);

        OP_FIELD_NAME = config.getString(OP_FIELD_CONFIG_NAME);
        SOURCE_TS_FIELD_NAME = config.getString(SOURCE_TS_FIELD_CONFIG_NAME);
        QUEUED_TS_FIELD_NAME = config.getString(QUEUED_TS_FIELD_CONFIG_NAME);
        UPDATE_MASK_FIELD_NAME = config.getString(UPDATE_MASK_FIELD_CONFIG_NAME);

        cdcSchemaCache = new SynchronizedCache<>(new LRUCache<>(16));
    }

    @Override
    public R apply(R record) {
        if (record.value() == null) {
            throw new DataException("Null record. "
                + "Tombstones should be dropped before reaching this transform.");
        }

        final Struct value = (Struct) record.value();

        final String operation = (String) value.get("op");

        Schema cdcSchema = cdcSchemaCache.get(value.schema());
        if (cdcSchema == null) {
            cdcSchema = makeCDCSchema(value.schema(), operation.equals("d"));
            cdcSchemaCache.put(value.schema(), cdcSchema);
        }

        Struct cdcValue = new Struct(cdcSchema);

        cdcValue.put(OP_FIELD_NAME, value.get("op"));
        cdcValue.put(QUEUED_TS_FIELD_NAME, value.get("ts_ms"));
        cdcValue.put(SOURCE_TS_FIELD_NAME, value.getStruct("source").get("ts_ms"));
        cdcValue.put(UPDATE_MASK_FIELD_NAME, operation.equals("u") ? makeUpdateMask(value) : null);

        Struct desiredStateValue = operatingState(value, operation);

        for (Field field : cdcSchema.fields()) {
            if (!isMetadataField(field.name())) {
                cdcValue.put(field, desiredStateValue.get(field.name()));
            }
        }

        return record.newRecord(
            record.topic(),
            record.kafkaPartition(),
            record.keySchema(),
            record.key(),
            cdcSchema,
            cdcValue,
            record.timestamp());
    }

    private String makeUpdateMask(Struct value) {
        Struct before = value.getStruct("before");
        Struct after = value.getStruct("after");
        List<String> updatedColumns = new ArrayList<>();

        for (Field field : after.schema().fields()) {
            try {
                if (before.get(field.name()) == null && after.get(field.name()) == null) {
                    continue;
                }
                else if (
                    (before.get(field.name()) == null && after.get(field.name()) != null) ||
                    (before.get(field.name()) != null && after.get(field.name()) == null)) {
                    updatedColumns.add(field.name());
                }
                else if (!after.get(field.name()).equals(before.get(field.name()))) {
                    updatedColumns.add(field.name());
                }
            }
            catch(Exception e) {
                LOGGER.error("Rhodry CDC Log Transform: Something went wrong while processing field: "
                    + field.name(), e);
            }
            // Should we check the reverse also?
            // Scenrios where a field has disappeared in the `after` section, 
            // e.g. records that came in after a drop column ddl.
            // Seems logical to include them, but there may repercussions in
            // SQL Server side. Ask Mohsen/Salar about this.
        }
        return String.join(",", updatedColumns);
    }

    private Schema makeCDCSchema(Schema schema, boolean fromBeforeState) {
        final Schema stateSchema = schema.field(fromBeforeState ? "before" : "after").schema();

        final SchemaBuilder builder = SchemaUtil.copySchemaBasics(stateSchema, SchemaBuilder.struct());

        builder.field(OP_FIELD_NAME, schema.field("op").schema());
        builder.field(QUEUED_TS_FIELD_NAME, schema.field("ts_ms").schema());
        builder.field(SOURCE_TS_FIELD_NAME, schema.field("source").schema().field("ts_ms").schema());
        builder.field(UPDATE_MASK_FIELD_NAME, Schema.OPTIONAL_STRING_SCHEMA);
        for (Field field : stateSchema.fields()) {
            builder.field(field.name(), field.schema());
        }

        return builder.build();
    }

    private Struct operatingState(Struct value, String operation) {
        if (operation.equals("c") ||
            operation.equals("r") ||
            operation.equals("u")) {
            return value.getStruct("after");
        } else if (operation.equals("d")) {
            return value.getStruct("before");
        }
        throw new DataException("No op field found. Is this a tombstone record?");
    }

    private boolean isMetadataField(String fieldName) {
        if (fieldName.equals(OP_FIELD_NAME) ||
            fieldName.equals(QUEUED_TS_FIELD_NAME) ||
            fieldName.equals(SOURCE_TS_FIELD_NAME) ||
            fieldName.equals(UPDATE_MASK_FIELD_NAME)) {
            return true;
        }
        return false;
    }

    @Override
    public ConfigDef config() {
        return CONFIG_DEF;
    }

    @Override
    public void close() {
        cdcSchemaCache = null;
    }    
}
