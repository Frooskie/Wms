using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseApiController<TEntity, TDto, TCreateDto, TUpdateDto>(
    IBaseService<TEntity> service,
    IMapper mapper) : ControllerBase
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly IMapper _mapper = mapper;

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await service.GetAllAsync(cancellationToken);
        var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var entity = await service.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return NotFound();
        var dto = _mapper.Map<TDto>(entity);
        return Ok(dto);
    }
    
    [HttpPost]
    [Authorize(Roles = "Manager,Chief")]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto createDto, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<TEntity>(createDto);
        await service.CreateAsync(entity, cancellationToken);
        var dto = _mapper.Map<TDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = GetEntityId(entity) }, dto);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Chief")]
    public virtual async Task<IActionResult> Update(int id, [FromBody] TUpdateDto updateDto, CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        _mapper.Map(updateDto, existing);
        await service.UpdateAsync(existing, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Chief")]
    public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    // Вспомогательный метод для получения Id сущности
    protected abstract object GetEntityId(TEntity entity);
}