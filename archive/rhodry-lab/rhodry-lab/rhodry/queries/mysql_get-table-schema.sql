SELECT
    TABLE_SCHEMA AS "database",
    TABLE_NAME AS "table",
    COLUMN_NAME AS "column",
    ORDINAL_POSITION AS "ordinal_position",
    IS_NULLABLE AS "nullable",
    DATA_TYPE AS "type",
    CHARACTER_MAXIMUM_LENGTH AS "length"
FROM
    INFORMATION_SCHEMA.COLUMNS
WHERE
    TABLE_NAME = '*|table|*'
ORDER BY
    ORDINAL_POSITION;
