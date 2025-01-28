package com.rhodry.connect.connectors.elasticsearch.elastic;

public class SslContextException extends RuntimeException {
    public SslContextException(Exception e) {
        super(e);
    }
}
