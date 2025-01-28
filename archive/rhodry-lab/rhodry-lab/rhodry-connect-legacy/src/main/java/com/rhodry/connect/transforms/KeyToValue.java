package com.rhodry.connect.transforms;

import org.apache.kafka.common.cache.Cache;
import org.apache.kafka.common.cache.LRUCache;
import org.apache.kafka.common.cache.SynchronizedCache;
import org.apache.kafka.common.config.ConfigDef;
import org.apache.kafka.connect.data.Field;
import org.apache.kafka.connect.data.Schema;
import org.apache.kafka.connect.data.SchemaBuilder;
import org.apache.kafka.connect.data.Struct;
import org.apache.kafka.connect.transforms.Transformation;
import org.apache.kafka.connect.connector.ConnectRecord;
import org.apache.kafka.connect.transforms.util.SchemaUtil;
import org.apache.kafka.connect.transforms.util.SimpleConfig;

import static org.apache.kafka.connect.transforms.util.Requirements.requireStructOrNull;
import static org.apache.kafka.connect.transforms.util.Requirements.requireStruct;

import java.util.Map;

/**
 * A custom Kafka connect Single-Message-Transform for extracting a field from the
 * header and inserting it as a field in the value.
 * At the moment, records with with schemas. It will be extended to include other
 * primitive types and nested fields in the future, as well as the support for schema-less records.
 * This SMT is similar to the original Kafka Connect's ValueToKey transform, but operates
 * in reverse.
 * It is intended to act on Debezium's Change Record Event messages, extract the id of
 * the record in the key, and include as a value field.
 */
public class KeyToValue<R extends ConnectRecord<R>> implements Transformation<R> {

    public static final String OVERVIEW_DOC = 
        "Extracts the record key and inserts it as a value.";
    
    private interface ConfigName {
        String VALUE_FIELD_NAME = "value.field.name";
    }

    private static final ConfigDef CONFIG_DEF = new ConfigDef()
        .define(
            ConfigName.VALUE_FIELD_NAME,
            ConfigDef.Type.STRING, "id",
            ConfigDef.Importance.MEDIUM,
            "Value field name as which to insert the extracted value.");

    private String valueFieldName;

    private Cache<Schema, Schema> schemaUpdateCache;

    private static final String PURPOSE = "adding key to record as a field.";

    @Override
    public void configure(Map<String, ?> props) {
        final SimpleConfig config = new SimpleConfig(CONFIG_DEF, props);
        valueFieldName = config.getString(ConfigName.VALUE_FIELD_NAME);

        schemaUpdateCache = new SynchronizedCache<>(new LRUCache<>(16));
    }

    @Override
    public R apply(R record) {
        if (record.valueSchema() == null) {
            return record;
        }

        final Struct value = requireStructOrNull(record.value(), PURPOSE);
        final Struct key = requireStruct(record.key(), PURPOSE);

        if (record.valueSchema() == null) {
            throw new IllegalStateException("KeyToValue SMT requires schema. No schema found.");
        }

        Schema updatedSchema = schemaUpdateCache.get(value.schema());
        if (updatedSchema == null) {
            updatedSchema = makeUpdatedSchema(record.keySchema(), record.valueSchema());
            schemaUpdateCache.put(value.schema(), updatedSchema);
        }

        final Struct updatedValue = new Struct(updatedSchema);

        for (Field field : record.valueSchema().fields()) {
            updatedValue.put(field.name(), value.get(field));
        }

        for (Field field : record.keySchema().fields()) {
            updatedValue.put(valueFieldName, key.get(field));
            break;
        }

        return record.newRecord(
            record.topic(),
            record.kafkaPartition(),
            record.keySchema(),
            record.key(),
            updatedSchema,
            updatedValue,
            record.timestamp());
    }

    private Schema makeUpdatedSchema(Schema keySchema, Schema valueSchema) {
        final SchemaBuilder builder = SchemaUtil.copySchemaBasics(valueSchema, SchemaBuilder.struct());

        for (Field field : valueSchema.fields()) {
            builder.field(field.name(), field.schema());
        }

        for (Field field : keySchema.fields()) {
            SchemaBuilder fieldSchemaBuilder = SchemaBuilder.type(field.schema().type()).optional();
            builder.field(valueFieldName, fieldSchemaBuilder.build());
            break;
        }

        return builder.build();
    }

    @Override
    public ConfigDef config() {
        return CONFIG_DEF;
    }

    @Override
    public void close() {
        schemaUpdateCache = null;
    }
}
