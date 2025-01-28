SELECT
    TABLE_NAME AS "table", 
    DATA_TYPE AS "pk_type"
FROM 
    information_schema.columns
    WHERE COLUMN_KEY = 'PRI'
    AND TABLE_SCHEMA = '*|db|*'
