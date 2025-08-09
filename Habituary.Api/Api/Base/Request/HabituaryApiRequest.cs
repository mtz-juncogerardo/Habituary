using Habituary.Core.Interfaces;
using MediatR;

namespace Habituary.Api.Request;

public class HabituaryApiRequest<TEntity> where TEntity : IEntity
{
    public record GetById(Guid IRN) : IRequest<TEntity>;
    public record Create(TEntity Entity) : IRequest<TEntity>;
    public record Update(TEntity Entity) : IRequest<TEntity>;
    public record Delete(Guid IRN) : IRequest<bool>;
    public record DeleteMany (IEnumerable<Guid> IRNs) : IRequest<bool>;
    public record GetAll() : IRequest<IEnumerable<TEntity>>;
}