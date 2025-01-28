CREATE TABLE ct.*|table|* (
    _pk INT IDENTITY(1, 1),
    _operation VARCHAR(1),
    _source_timestamp_utc DATETIME2,
    _queued_timestamp_utc DATETIME2,
    _destination_timestamp_utc DATETIME2 DEFAULT(GETUTCDATE()),
    id BIGINT,
    INDEX ix_*|table|*_source_timestamp_utc NONCLUSTERED (_source_timestamp_utc) ON FG_INDEX, 
    CONSTRAINT PK_*|table|* PRIMARY KEY CLUSTERED (_pk) 
) ON FG_REGULAR;
