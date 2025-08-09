using System.Reflection;
using Habituary.Core.Interfaces;
using Habituary.Data.Models;

namespace Habituary.Data.Mapper;

public static class EntityRecordMapper<TRecord, TEntity>
    where TRecord : BaseIORecord, new()
    where TEntity : IEntity, new()
{
    public static TRecord MapToRecord(TEntity entity, object? extraProperties = null)
    {
        var record = new TRecord();
        MapProperties(entity, record, extraProperties, true);
        // Mapear propiedades de navegación (entidades hijas)
        var entityProps = typeof(TEntity).GetProperties();
        var recordProps = typeof(TRecord).GetProperties();
        foreach (var prop in entityProps)
        {
            if (!typeof(IEntity).IsAssignableFrom(prop.PropertyType) || prop.GetValue(entity) == null) continue;
            var recordProp = recordProps.FirstOrDefault(p => p.Name == prop.Name);
            if (recordProp == null || !typeof(BaseIORecord).IsAssignableFrom(recordProp.PropertyType)) continue;
            var method = typeof(EntityRecordMapper<,>)
                .MakeGenericType(recordProp.PropertyType, prop.PropertyType)
                .GetMethod("MapToRecord");
            var mapped = method.Invoke(null, new[] { prop.GetValue(entity), null });
            recordProp.SetValue(record, mapped);
        }
        return record;
    }

    public static TEntity MapToEntity(TRecord record, object? extraProperties = null)
    {
        var entity = new TEntity();
        MapProperties(record, entity, extraProperties, false);
        // Mapear propiedades de navegación (entidades hijas)
        var recordProps = typeof(TRecord).GetProperties();
        var entityProps = typeof(TEntity).GetProperties();
        foreach (var prop in recordProps)
        {
            if (!typeof(BaseIORecord).IsAssignableFrom(prop.PropertyType) || prop.GetValue(record) == null) continue;
            var entityProp = entityProps.FirstOrDefault(p => p.Name == prop.Name);
            if (entityProp == null || !typeof(IEntity).IsAssignableFrom(entityProp.PropertyType)) continue;
            var method = typeof(EntityRecordMapper<,>)
                .MakeGenericType(prop.PropertyType, entityProp.PropertyType)
                .GetMethod("MapToEntity");
            var mapped = method.Invoke(null, new[] { prop.GetValue(record), null });
            entityProp.SetValue(entity, mapped);
        }
        return entity;
    }

    private static void MapProperties(object source, object target, object? extras, bool isEntityToRecord)
    {
        var sourceProperties = source.GetType().GetProperties();
        var targetProperties = target.GetType().GetProperties();

        foreach (var targetProp in targetProperties)
        {
            if (TrySetFromExtras(target, targetProp, extras)) continue;

            var sourceProp = sourceProperties.FirstOrDefault(p => p.Name == targetProp.Name);
            if (sourceProp == null) continue;

            if (IsIRNMapping(sourceProp, targetProp))
            {
                SetIRNValue(source, sourceProp, target, targetProp);
                continue;
            }

            if (IsNestedMapping(sourceProp, targetProp))
            {
                MapNested(source, sourceProp, target, targetProp, isEntityToRecord);
                continue;
            }

            if (sourceProp.PropertyType == targetProp.PropertyType)
                targetProp.SetValue(target, sourceProp.GetValue(source));
        }
    }

    private static bool TrySetFromExtras(object target, PropertyInfo targetProp, object? extras)
    {
        if (extras == null) return false;

        var extraProp = extras.GetType().GetProperty(targetProp.Name);
        if (extraProp == null) return false;

        var value = extraProp.GetValue(extras);
        if (AreGuidAndStringCompatible(extraProp.PropertyType, targetProp.PropertyType) && value != null)
        {
            if (targetProp.PropertyType == typeof(Guid))
                targetProp.SetValue(target, Guid.Parse(value.ToString()));
            else
                targetProp.SetValue(target, value.ToString());
        }
        else
        {
            targetProp.SetValue(target, value);
        }

        return true;
    }

    private static bool IsIRNMapping(PropertyInfo sourceProp, PropertyInfo targetProp)
    {
        return targetProp.Name == nameof(BaseIORecord.IRN) &&
               (sourceProp.PropertyType == typeof(string) || sourceProp.PropertyType == typeof(Guid)) &&
               (targetProp.PropertyType == typeof(Guid) || targetProp.PropertyType == typeof(string));
    }

    private static void SetIRNValue(object source, PropertyInfo sourceProp, object target, PropertyInfo targetProp)
    {
        var value = sourceProp.GetValue(source);
        if (value == null) return;

        if (targetProp.PropertyType == typeof(Guid))
            targetProp.SetValue(target, Guid.Parse(value.ToString()));
        else
            targetProp.SetValue(target, value.ToString());
    }

    private static bool IsNestedMapping(PropertyInfo sourceProp, PropertyInfo targetProp)
    {
        return typeof(IEntity).IsAssignableFrom(sourceProp.PropertyType) &&
               typeof(BaseIORecord).IsAssignableFrom(targetProp.PropertyType);
    }

    private static void MapNested(object source, PropertyInfo sourceProp, object target, PropertyInfo targetProp,
        bool isEntityToRecord)
    {
        var nestedValue = sourceProp.GetValue(source);
        if (nestedValue == null) return;

        var genericMapper = typeof(EntityRecordMapper<,>).MakeGenericType(
            isEntityToRecord ? targetProp.PropertyType : sourceProp.PropertyType,
            isEntityToRecord ? sourceProp.PropertyType : targetProp.PropertyType
        );

        var method = genericMapper.GetMethod(isEntityToRecord ? "MapToRecord" : "MapToEntity");
        var mappedNested = method?.Invoke(null, new[] { nestedValue, null });
        targetProp.SetValue(target, mappedNested);
    }

    private static bool AreGuidAndStringCompatible(Type type1, Type type2)
    {
        return (type1 == typeof(Guid) && type2 == typeof(string)) ||
               (type1 == typeof(string) && type2 == typeof(Guid));
    }
}