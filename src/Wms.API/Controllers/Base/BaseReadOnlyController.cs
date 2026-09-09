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
[Produces("application/json")]
public abstract class BaseReadOnlyController<TEntity, TDto>(IReadOnlyService<TEntity> service, IMapper mapper)
    : ControllerBase
    where TEntity : class
    where TDto : class
{
    protected readonly IMapper Mapper = mapper;

    /// <summary>Получить все сущности.</summary>
    /// <response code="200">Список сущностей.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await service.GetAllAsync(cancellationToken);
        return Ok(Mapper.Map<IEnumerable<TDto>>(entities));
    }

    /// <summary>Получить сущность по идентификатору.</summary>
    /// <param name="id">Идентификатор.</param>
    /// <response code="200">Сущность найдена.</response>
    /// <response code="404">Сущность не найдена.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public virtual async Task<ActionResult<TDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var entity = await service.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return NotFound();
        return Ok(Mapper.Map<TDto>(entity));
    }

    /// <summary>Получить страницу сущностей с пагинацией.</summary>
    /// <param name="request">Параметры пагинации.</param>
    /// <response code="200">Страница сущностей.</response>
    [HttpGet("paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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