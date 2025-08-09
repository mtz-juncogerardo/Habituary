using Habituary.Api.Request;
using Habituary.Core.Interfaces;
using Habituary.Data.Context;
using Habituary.Data.Models;
using MediatR;

namespace Habituary.Api.Repository;

public abstract class HabituaryApiHandler<TEntity, TRecord> :
    IRequestHandler<HabituaryApiRequest<TEntity>.GetById, TEntity>,
    IRequestHandler<HabituaryApiRequest<TEntity>.Create, TEntity>,
    IRequestHandler<HabituaryApiRequest<TEntity>.Update, TEntity>,
    IRequestHandler<HabituaryApiRequest<TEntity>.Delete, bool>,
    IRequestHandler<HabituaryApiRequest<TEntity>.DeleteMany, bool>,
    IRequestHandler<HabituaryApiRequest<TEntity>.GetAll, IEnumerable<TEntity>>
    where TEntity : IEntity, new()
    where TRecord : BaseIORecord, new()
{
    private HabituaryRepository<TEntity, TRecord> _repository;
    protected readonly ICurrentUser _currentUser;
    protected readonly HabituaryDbContext _dbContext;
    
    public HabituaryApiHandler(HabituaryDbContext context, ICurrentUser currentUser)
    {
        _repository = new  HabituaryRepository<TEntity, TRecord>(context, currentUser);
        _currentUser = currentUser;
        _dbContext = context;
    }

    public Task<TEntity> Handle(HabituaryApiRequest<TEntity>.GetById request, CancellationToken cancellationToken)
    {
        ValidateRequest(request.IRN.ToString());
        return HandleGetById(request, cancellationToken);
    }
    public virtual Task<TEntity> HandleGetById(HabituaryApiRequest<TEntity>.GetById request, CancellationToken cancellationToken)
    {
        return _repository.GetByIdAsync(request.IRN);
    }

    public Task<TEntity> Handle(HabituaryApiRequest<TEntity>.Create request, CancellationToken cancellationToken)
    {
        ValidateRequest(null, false);
        return HandleCreate(request, cancellationToken);
    }
    public virtual Task<TEntity> HandleCreate(HabituaryApiRequest<TEntity>.Create request, CancellationToken cancellationToken)
    {
        return _repository.CreateAsync(request.Entity);
    }

    public Task<TEntity> Handle(HabituaryApiRequest<TEntity>.Update request, CancellationToken cancellationToken)
    {
        ValidateRequest(request.Entity.IRN);
        return HandleUpdate(request, cancellationToken);
    }
    public virtual Task<TEntity> HandleUpdate(HabituaryApiRequest<TEntity>.Update request, CancellationToken cancellationToken)
    {
        return _repository.UpdateAsync(request.Entity);
    }

    public Task<bool> Handle(HabituaryApiRequest<TEntity>.Delete request, CancellationToken cancellationToken)
    {
        ValidateRequest(request.IRN.ToString());
        return HandleDelete(request, cancellationToken);
    }
    public virtual Task<bool> HandleDelete(HabituaryApiRequest<TEntity>.Delete request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.IRN);
    }

    public Task<bool> Handle(HabituaryApiRequest<TEntity>.DeleteMany request, CancellationToken cancellationToken)
    {
        foreach (var irn in request.IRNs)
        {
            ValidateRequest(irn.ToString());
        }
        return HandleDeleteMany(request, cancellationToken);
    }
    public virtual Task<bool> HandleDeleteMany(HabituaryApiRequest<TEntity>.DeleteMany request, CancellationToken cancellationToken)
    {
        return _repository.DeleteManyAsync(request.IRNs);
    }

    public Task<IEnumerable<TEntity>> Handle(HabituaryApiRequest<TEntity>.GetAll request, CancellationToken cancellationToken)
    {
        ValidateRequest(null, false);
        return HandleGetAll(request, cancellationToken);
    }
    public virtual Task<IEnumerable<TEntity>> HandleGetAll(HabituaryApiRequest<TEntity>.GetAll request, CancellationToken cancellationToken)
    {
        return _repository.GetAllAsync();
    }

    private void ValidateRequest(string? requestIrn, bool validateIRNFlag = true)
    {
        if (!HasPermission(requestIrn, validateIRNFlag))
        {
            throw new UnauthorizedAccessException("You do not have permission to access this resource.");
        }
    }
    
    public abstract bool HasPermission(string? requestIrn, bool validateIRNFlag);
}