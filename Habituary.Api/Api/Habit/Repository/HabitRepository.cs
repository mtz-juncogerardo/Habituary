using Habituary.Api.Habit.Entities;
using Habituary.Core.Interfaces;
using Habituary.Data.Context;
using Habituary.Data.Mapper;
using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api.Habit.Repository;

public class HabitRepository
{
    private readonly HabituaryDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public HabitRepository(HabituaryDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<HabitEntity?> GetById(Guid irn)
    {
        var record = await _dbContext.Habits.FirstOrDefaultAsync(r => r.IRN == irn);
        if (record == null || record.UserIRN != _currentUser.IRN)
            return null;
        return EntityRecordMapper<HabitRecord, HabitEntity>.MapToEntity(record);
    }
}

