using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Common;
using Wms.API.Extensions;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseCrudControllerWithValidation<TEntity, TDto, TCreateDto, TUpdateDto>(
    ICrudService<TEntity> service,
    IMapper mapper,
    IValidator<TCreateDto>? createValidator = null,
    IValidator<TUpdateDto>? updateValidator = null)
    : BaseCrudController<TEntity, TDto, TCreateDto, TUpdateDto>(service, mapper)
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>Создать новую сущность с валидацией.</summary>
    /// <param name="createDto">Данные для создания.</param>
    /// <response code="201">Сущность создана. Возвращает созданный объект.</response>
    /// <response code="400">Ошибка валидации.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WmsProblemDetails))]
    public override async Task<ActionResult<TDto>> Create(TCreateDto createDto, CancellationToken cancellationToken)
    {
        if (createValidator != null)
            await createValidator.ValidateAndThrowAsync(createDto, cancellationToken);
        return await base.Create(createDto, cancellationToken);
    }

    /// <summary>Обновить сущность с валидацией.</summary>
    /// <param name="id">Идентификатор.</param>
    /// <param name="updateDto">Данные для обновления.</param>
    /// <response code="204">Обновление выполнено успешно.</response>
    /// <response code="400">Ошибка валидации.</response>
    /// <response code="404">Сущность не найдена.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(WmsProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(WmsProblemDetails))]
    public override async Task<IActionResult> Update(int id, TUpdateDto updateDto, CancellationToken cancellationToken)
    {
        if (updateValidator != null)
            await updateValidator.ValidateAndThrowAsync(updateDto, cancellationToken);
        return await base.Update(id, updateDto, cancellationToken);
    }
}