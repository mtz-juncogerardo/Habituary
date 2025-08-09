using Habituary.Core.Extensions;
using Habituary.Core.Interfaces;
using Habituary.Data.Context;
using Habituary.Data.Mapper;
using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api.Repository;

public sealed class HabituaryRepository<TEntity, TRecord>
    where TEntity : IEntity, new()
    where TRecord : BaseIORecord, new()
{
    private readonly DbContext _context;
    private readonly DbSet<TRecord> _dbSet;
    private readonly ICurrentUser _currentUser;

    public HabituaryRepository(HabituaryDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _dbSet = _context.Set<TRecord>();
        _currentUser = currentUser;
    }

    public async Task<TEntity> GetByIdAsync(Guid irn)
    {
        var query = _dbSet.AsQueryable();
        // Incluir todas las propiedades de navegación (tablas foráneas)
        var navigationProperties = typeof(TRecord).GetProperties()
            .Where(p => typeof(BaseIORecord).IsAssignableFrom(p.PropertyType) ||
                        (p.PropertyType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition())));
        foreach (var navProp in navigationProperties)
        {
            query = query.Include(navProp.Name);
        }
        var record = await query.FirstOrDefaultAsync(r => EF.Property<Guid>(r, "IRN") == irn);
        return record == null ? new TEntity() : EntityRecordMapper<TRecord, TEntity>.MapToEntity(record);
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        var record = EntityRecordMapper<TRecord, TEntity>.MapToRecord(entity);
        record.SetAudit(_currentUser.Email);
        _dbSet.Add(record);
        await _context.SaveChangesAsync();
        return EntityRecordMapper<TRecord, TEntity>.MapToEntity(record);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var record = EntityRecordMapper<TRecord, TEntity>.MapToRecord(entity);
        record.UpdateExisting(_currentUser.Email);
        _dbSet.Update(record);
        await _context.SaveChangesAsync();
        return EntityRecordMapper<TRecord, TEntity>.MapToEntity(record);
    }

    public async Task<bool> DeleteAsync(Guid irn)
    {
        var record = await _dbSet.FindAsync(irn);
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record), "Record not found");
        }
        _dbSet.Remove(record);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteManyAsync(IEnumerable<Guid> irns)
    {
        var records = await _dbSet.Where(r => irns.Contains(r.IRN)).ToListAsync();
        if (records.Count == 0)
        {
            throw new ArgumentNullException(nameof(records), "No records found to delete");
        }
        _dbSet.RemoveRange(records);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var userIrnProp = typeof(TRecord).GetProperty("UserIRN");
        if (userIrnProp == null)
            return Enumerable.Empty<TEntity>();
        var query = _dbSet.AsQueryable();
        var navigationProperties = typeof(TRecord).GetProperties()
            .Where(p => typeof(BaseIORecord).IsAssignableFrom(p.PropertyType) ||
                        (p.PropertyType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition())));
        foreach (var navProp in navigationProperties)
        {
            query = query.Include(navProp.Name);
        }
        var records = await query.Where(r => EF.Property<Guid>(r, "UserIRN") == _currentUser.IRN).ToListAsync();
        return records.Select(r => EntityRecordMapper<TRecord, TEntity>.MapToEntity(r));
    }
}