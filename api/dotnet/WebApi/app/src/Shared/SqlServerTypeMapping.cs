using System.Data;


namespace WebApi.Shared;

public static class SqlServerTypeMapping
{
    public static DbType AsDbType(this SqlDbType tsqlType) => tsqlType switch
    {
        SqlDbType.BigInt => DbType.Int64,
        SqlDbType.Binary => DbType.Binary,
        SqlDbType.Bit => DbType.Boolean,
        SqlDbType.Char => DbType.AnsiStringFixedLength,
        SqlDbType.DateTime => DbType.DateTime,
        SqlDbType.Decimal => DbType.Decimal,
        SqlDbType.Float => DbType.Double,
        SqlDbType.Image => DbType.Binary,
        SqlDbType.Int => DbType.Int32,
        SqlDbType.Money => DbType.Decimal,
        SqlDbType.NChar => DbType.StringFixedLength,
        SqlDbType.NText => DbType.String,
        SqlDbType.NVarChar => DbType.String,
        SqlDbType.Real => DbType.Single,
        SqlDbType.UniqueIdentifier => DbType.Guid,
        SqlDbType.SmallDateTime => DbType.DateTime,
        SqlDbType.SmallInt => DbType.Int16,
        SqlDbType.SmallMoney => DbType.Decimal,
        SqlDbType.Text => DbType.String,
        SqlDbType.Timestamp => DbType.Binary,
        SqlDbType.TinyInt => DbType.Byte,
        SqlDbType.VarBinary => DbType.Binary,
        SqlDbType.VarChar => DbType.String,
        SqlDbType.Variant => DbType.Object,
        SqlDbType.Xml => DbType.Xml,
        SqlDbType.Udt => throw new NotImplementedException(),
        SqlDbType.Structured => throw new NotImplementedException(),
        SqlDbType.Date => DbType.Date,
        SqlDbType.Time => DbType.Time,
        SqlDbType.DateTime2 => DbType.DateTime2,
        SqlDbType.DateTimeOffset => DbType.DateTimeOffset,
        _ => throw new NotImplementedException()
    };
}
