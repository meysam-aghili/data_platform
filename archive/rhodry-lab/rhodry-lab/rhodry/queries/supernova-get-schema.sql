SELECT 
	ic.Table_Name AS table_name,
	ic.Column_Name AS column_name,
	ic.data_Type AS data_type,
	ic.Character_Maximum_Length AS max_length,
	ic.Numeric_precision AS `precision`,
	ic.numeric_scale AS scale,
	ic.is_nullable AS is_nullable, 
	ic.ordinal_position AS ordinal_position
FROM 
    INFORMATION_SCHEMA.COLUMNS AS ic
WHERE 
    ic.TABLE_SCHEMA = '*|db|*'