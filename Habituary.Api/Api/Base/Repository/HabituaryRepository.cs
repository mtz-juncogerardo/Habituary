using Habituary.Core.Extensions;
using Habituary.Core.Interfaces;
using Habituary.Data.Context;
using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api.Repository;

public sealed class HabituaryRepository<TEntity, TRecord>
    where TEntity : IEntity
    where TRecord : BaseIORecord, new()
{
    public readonly DbContext _context;
    public readonly DbSet<TRecord> _dbSet;
    public readonly ICurrentUser _currentUser;

    public HabituaryRepository(HabituaryDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _dbSet = _context.Set<TRecord>();
        _currentUser = currentUser;
    }

    public async Task<TEntity> GetByIdAsync(Guid irn)
    {
        var record = await _dbSet.FindAsync(irn);
        return (TEntity)(object)record;
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        var record = (TRecord)(object)entity;
        record.SetAudit(_currentUser.Email);
        _dbSet.Add(record);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var record = (TRecord)(object)entity;
        record.UpdateExisting(_currentUser.Email);
        _dbSet.Update(record);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid irn)
    {
        var record = await _dbSet.FindAsync(irn);
        if (record == null) return false;
        _dbSet.Remove(record);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteManyAsync(IEnumerable<Guid> irns)
    {
        var records = await _dbSet.Where(r => irns.Contains(r.IRN)).ToListAsync();
        if (!records.Any()) return false;
        _dbSet.RemoveRange(records);
        await _context.SaveChangesAsync();
        return true;
    }
}