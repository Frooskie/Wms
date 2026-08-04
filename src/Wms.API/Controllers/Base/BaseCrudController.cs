using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseCrudController<TEntity, TDto, TCreateDto, TUpdateDto>(
    ICrudService<TEntity> service,
    IMapper mapper) : BaseReadOnlyController<TEntity, TDto>(service, mapper)
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    [HttpPost]
    [Authorize(Roles = "Manager,Chief")]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto createDto,
        CancellationToken cancellationToken)
    {
        var entity = Mapper.Map<TEntity>(createDto);
        var created = await service.CreateAsync(entity, cancellationToken);
        var dto = Mapper.Map<TDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = GetEntityId(created) }, dto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Chief")]
    public virtual async Task<IActionResult> Update(int id, [FromBody] TUpdateDto updateDto,
        CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound();

        Mapper.Map(updateDto, existing);
        await service.UpdateAsync(existing, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Chief")]
    public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound();

        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    protected abstract object GetEntityId(TEntity entity);
}