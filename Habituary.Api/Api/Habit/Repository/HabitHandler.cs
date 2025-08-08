using Habituary.Api.Habit.Entities;
using Habituary.Data.Context;
using Habituary.Data.Models;
using Habituary.Core.Interfaces;
using Habituary.Api.Repository;
using Habituary.Api.Request;

namespace Habituary.Api.Habit.Repository
{
    public class HabitHandler : HabituaryApiHandler<HabitEntity, HabitRecord>
    {
        private readonly HabitRepository _habitRepository;

        public HabitHandler(HabituaryDbContext context, ICurrentUser currentUser, HabitRepository habitRepository)
            : base(context, currentUser)
        {
            _habitRepository = habitRepository;
        }

        public override async Task<HabitEntity> Handle(HabituaryApiRequest<HabitEntity>.GetById request, CancellationToken cancellationToken)
        {
            return await _habitRepository.GetById(request.IRN);
        }
    }
}
