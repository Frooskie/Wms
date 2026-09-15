using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.Constants;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public abstract class BaseCrudController<TEntity, TDto, TCreateDto, TUpdateDto>(
    ICrudService<TEntity> service,
    IMapper mapper) : BaseReadOnlyController<TEntity, TDto>(service, mapper)
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>Создать новую сущность.</summary>
    /// <param name="createDto">Данные для создания.</param>
    /// <response code="201">Сущность создана. Возвращает созданный объект.</response>
    /// <response code="400">Ошибка валидации.</response>
    [HttpPost]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public virtual async Task<ActionResult<TDto>> Create(
        [FromBody] TCreateDto createDto,
        CancellationToken cancellationToken)
    {
        var entity = Mapper.Map<TEntity>(createDto);
        var created = await service.CreateAsync(entity, cancellationToken);
        var dto = Mapper.Map<TDto>(created);
        
        return CreatedAtAction(nameof(GetById), new { id = GetEntityId(created) }, dto);
    }

    /// <summary>Обновить сущность.</summary>
    /// <response code="204">Обновление выполнено успешно.</response>
    /// <response code="400">Ошибка валидации.</response>
    /// <response code="404">Сущность не найдена.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public virtual async Task<IActionResult> Update(
        int id,
        [FromBody] TUpdateDto updateDto,
        CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            throw new NotFoundException(typeof(TEntity).Name, id);

        Mapper.Map(updateDto, existing);
        await service.UpdateAsync(existing, cancellationToken);
        
        return NoContent();
    }

    /// <summary>Удалить сущность.</summary>
    /// <param name="id">Идентификатор.</param>
    /// <response code="204">Удаление выполнено успешно.</response>
    /// <response code="404">Сущность не найдена.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Manager + "," + Roles.Chief)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await service.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            throw new NotFoundException(typeof(TEntity).Name, id);

        await service.DeleteAsync(id, cancellationToken);
        
        return NoContent();
    }

    protected abstract object GetEntityId(TEntity entity);
}