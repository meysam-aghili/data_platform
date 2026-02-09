CREATE DATABASE DW

USE DW

CREATE SCHEMA api

DROP PROC api.sp_sales_target_values
CREATE PROC [api].[sp_sales_target_values] @PageNumber int, @PageSize int
AS
	BEGIN
		WITH Numbers AS 
		(
			SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS id
			FROM master.dbo.spt_values
		),
		final AS
		(
			SELECT 
				id,
				'test' + CAST(id AS VARCHAR) AS name,
				GETDATE() AS datetime_
			FROM Numbers
		)
		SELECT TOP (@PageSize)
			*
		FROM final
		WHERE id BETWEEN (@PageNumber-1)*@PageSize AND @PageNumber*@PageSize
	END

CREATE LOGIN [api] WITH PASSWORD = 'testpass';
CREATE USER api FOR LOGIN api;
ALTER ROLE db_datareader ADD MEMBER api;
GRANT EXEC TO [api];
GRANT CREATE TABLE, CREATE VIEW, CREATE FUNCTION, CREATE PROCEDURE TO [api];

ALTER ROLE db_owner ADD MEMBER [api];

GO

SELECT * FROM api.api_log where responsetime is not null;