using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseReadOnlyController<TEntity, TDto>(IReadOnlyService<TEntity> service, IMapper mapper)
    : ControllerBase
    where TEntity : class
    where TDto : class
{
    protected readonly IMapper Mapper = mapper;

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await service.GetAllAsync(cancellationToken);
        return Ok(Mapper.Map<IEnumerable<TDto>>(entities));
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var entity = await service.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return NotFound();
        return Ok(Mapper.Map<TDto>(entity));
    }
}