using Habituary.Api.Mood.Entities;
using Habituary.Api.Repository;
using Habituary.Data.Context;
using Habituary.Data.Models;
using Habituary.Core.Interfaces;

namespace Habituary.Api.Mood.Repository
{
    public class MoodHandler : HabituaryApiHandler<MoodEntity, MoodRecord>
    {
        public MoodHandler(HabituaryDbContext context, ICurrentUser currentUser) : base(context, currentUser)
        {
        }

        public override bool HasPermission(string? requestIrn, bool validateIRNFlag)
        {
            if (!validateIRNFlag) return true;
            if (requestIrn == null) return false;
            var mood = _dbContext.Moods.FirstOrDefault(r =>
                r.IRN.ToString() == requestIrn && r.UserIRN == _currentUser.IRN);
            return mood != null;
        }
    }
}
