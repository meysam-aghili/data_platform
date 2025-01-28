WITH [rows] AS (
    SELECT
        sOBJ.name AS [table],
        SUM(sPTN.Rows) AS [rows]
    FROM 
        sys.objects AS sOBJ WITH(NOLOCk)
        INNER JOIN sys.partitions AS sPTN WITH(NOLOCk) ON sOBJ.object_id = sPTN.object_id
    WHERE
        sOBJ.type = 'U'
        AND sOBJ.is_ms_shipped = 0x0
        AND index_id < 2
        AND SCHEMA_NAME(sOBJ.schema_id) = '*|schema|*'
    GROUP BY
        sOBJ.schema_id,
        sOBJ.name
)
SELECT * FROM [rows];
