using Habituary.Api.HabitLog.Entities;
using Habituary.Api.Repository;
using Habituary.Data.Context;
using Habituary.Data.Models;
using Habituary.Core.Interfaces;

namespace Habituary.Api.HabitLog.Repository
{
    public class HabitLogHandler : HabituaryApiHandler<HabitLogEntity, HabitLogRecord>
    {
        public HabitLogHandler(HabituaryDbContext context, ICurrentUser currentUser) : base(context, currentUser)
        {
        }
    }
}
