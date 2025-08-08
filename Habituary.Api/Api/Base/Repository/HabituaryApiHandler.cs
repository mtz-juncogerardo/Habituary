using Habituary.Api.Request;
using Habituary.Core.Interfaces;
using Habituary.Data.Context;
using Habituary.Data.Models;
using MediatR;

namespace Habituary.Api.Repository;

public class HabituaryApiHandler<TEntity, TRecord> :
    IRequestHandler<HabituaryApiRequest<TEntity>.GetById, TEntity>,
    IRequestHandler<HabituaryApiRequest<TEntity>.Create, TEntity>,
    IRequestHandler<HabituaryApiRequest<TEntity>.Update, TEntity>,
    IRequestHandler<HabituaryApiRequest<TEntity>.Delete, bool>,
    IRequestHandler<HabituaryApiRequest<TEntity>.DeleteMany, bool>
    where TEntity : IEntity
    where TRecord : BaseIORecord, new()
{
    private HabituaryRepository<TEntity, TRecord> _repository;
    
    public HabituaryApiHandler(HabituaryDbContext context, ICurrentUser currentUser)
    {
        _repository = new  HabituaryRepository<TEntity, TRecord>(context, currentUser);
    }

    public virtual async Task<TEntity> Handle(HabituaryApiRequest<TEntity>.GetById request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IRN);
    }

    public virtual async Task<TEntity> Handle(HabituaryApiRequest<TEntity>.Create request, CancellationToken cancellationToken)
    {
        return await _repository.CreateAsync(request.Entity);
    }

    public virtual async Task<TEntity> Handle(HabituaryApiRequest<TEntity>.Update request, CancellationToken cancellationToken)
    {
        return await _repository.UpdateAsync(request.Entity);
    }

    public virtual async Task<bool> Handle(HabituaryApiRequest<TEntity>.Delete request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.IRN);
    }

    public virtual async Task<bool> Handle(HabituaryApiRequest<TEntity>.DeleteMany request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteManyAsync(request.IRNs);
    }
}