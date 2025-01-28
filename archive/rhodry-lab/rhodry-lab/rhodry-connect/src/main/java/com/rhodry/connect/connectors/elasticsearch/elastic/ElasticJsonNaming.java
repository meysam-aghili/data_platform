package com.rhodry.connect.connectors.elasticsearch.elastic;

public class ElasticJsonNaming {
    public static String removeKeywordSuffix(String fieldName) {
        return fieldName == null ? null : fieldName.replace(".keyword", "");
    }
}
