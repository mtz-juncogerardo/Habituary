using Habituary.Api.Request;
using Habituary.Core.Interfaces;
using Habituary.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Habituary.Api;

public class HabituaryApiControllerBase<TEntity> : BaseController 
    where TEntity : IEntity
{
    public HabituaryApiControllerBase(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("{irn}")]
    public async Task<TEntity> GetById(Guid irn)
    {
        return await Mediator.Send(new HabituaryApiRequest<TEntity>.GetById(irn));
    }
    
    [HttpPost]
    public async Task<TEntity> Create([FromBody] TEntity entity)
    {
        return await Mediator.Send(new HabituaryApiRequest<TEntity>.Create(entity));
    }

    [HttpPut("{irn}")]
    public async Task<TEntity> Update(Guid irn, [FromBody] TEntity entity)
    {
        return await Mediator.Send(new HabituaryApiRequest<TEntity>.Update(entity));
    }
    
    [HttpDelete("{irn}")]
    public async Task<bool> Delete(Guid irn)
    {
        return await Mediator.Send(new HabituaryApiRequest<TEntity>.Delete(irn));
    }
    
    [HttpPost("DeleteMany")]
    public async Task<bool> DeleteMany([FromBody] IEnumerable<Guid> irns)
    {
        return await Mediator.Send(new HabituaryApiRequest<TEntity>.DeleteMany(irns));
    }
}