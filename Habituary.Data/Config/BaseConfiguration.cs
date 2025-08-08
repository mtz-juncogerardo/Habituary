using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Habituary.Data.Config;

public static class BaseConfiguration
{
    public static void InitialConfig(ModelBuilder modelBuilder)
    {
        SetGuidConfiguration(modelBuilder);
        SetDateConfiguration(modelBuilder);
        SetStringConfiguration(modelBuilder);
        SetDecimalConfiguration(modelBuilder);
        SetEnumConfiguration(modelBuilder);
    }

    private static void SetDecimalConfiguration(ModelBuilder modelBuilder)
    {
        var decimalProperties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

        foreach (var property in decimalProperties)
        {
            property.SetDefaultValueSql("0");
            property.SetColumnType("numeric(28, 10)");
        }
    }

    private static void SetEnumConfiguration(ModelBuilder modelBuilder)
    {
        var enumProperties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType.IsEnum);

        foreach (var property in enumProperties)
        {
            var enumType = property.ClrType;
            var converter = new ValueConverter<int, int>(
                v => Convert.ToInt32(v),
                v => (int)Enum.ToObject(enumType, v)
            );
            property.SetDefaultValueSql("0");
            property.SetColumnType("int");
            property.SetValueConverter(converter);
        }
    }

    private static void SetGuidConfiguration(ModelBuilder modelBuilder)
    {
        var guidProperties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(Guid));

        foreach (var property in guidProperties)
        {
            property.SetDefaultValueSql(null);
            property.SetColumnType("uuid");
        }
    }

    private static void SetStringConfiguration(ModelBuilder modelBuilder)
    {
        var stringProperties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(string));

        foreach (var property in stringProperties) property.SetColumnType("varchar(50)");
    }

    private static void SetDateConfiguration(ModelBuilder modelBuilder)
    {
        var dateTimeProperties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

        foreach (var property in dateTimeProperties)
        {
            property.SetDefaultValueSql("TIMESTAMP '1899-10-30 00:00:00'");
            property.SetColumnType("timestamp with time zone");
            property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            ));
        }
    }
}