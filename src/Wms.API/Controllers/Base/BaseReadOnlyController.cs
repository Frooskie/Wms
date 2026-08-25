using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.DTOs.common;
using Wms.Core.DTOs.Common;
using Wms.Core.Interfaces.Services.Base;

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

    [HttpGet("paged")]
    public virtual async Task<ActionResult<PagedResult<TDto>>> GetPaged(
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedAsync(
            filter: null,
            orderBy: null,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoItems = Mapper.Map<IEnumerable<TDto>>(result.Items);

        var pagedResult = new PagedResult<TDto>(
            dtoItems,
            result.TotalCount,
            result.PageNumber,
            result.PageSize
        );

        return Ok(pagedResult);
    }
}