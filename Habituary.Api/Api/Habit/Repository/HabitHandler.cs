using Habituary.Data.Context;
using Habituary.Data.Models;
using Habituary.Core.Interfaces;
using Habituary.Api.Repository;
using Habituary.Api.Request;
using Habituary.Core.Entities;

namespace Habituary.Api.Habit.Repository;

public class HabitHandler : HabituaryApiHandler<HabitEntity, HabitRecord>
{
    private readonly HabitRepository _habitRepository;

    public HabitHandler(HabituaryDbContext context, ICurrentUser currentUser, HabitRepository habitRepository)
        : base(context, currentUser)
    {
        _habitRepository = habitRepository;
    }

    public override Task<HabitEntity> HandleGetById(HabituaryApiRequest<HabitEntity>.GetById request, CancellationToken cancellationToken)
    {
        return _habitRepository.GetById(request.IRN);
    }

    public override bool HasPermission(string? requestIrn, bool validateIRNFlag)
    {
        if (!validateIRNFlag) return true;
        if (requestIrn == null) return false;
        var habit = _dbContext.Habits.FirstOrDefault(r =>
            r.IRN.ToString() == requestIrn && r.UserIRN == _currentUser.IRN);
        return habit != null;
    }
}