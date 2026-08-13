using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Extensions;
using Wms.Core.Interfaces.Services.Base;

namespace Wms.API.Controllers.Base;

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
    [HttpPost]
    public override async Task<ActionResult<TDto>> Create(TCreateDto createDto, CancellationToken cancellationToken)
    {
        if (createValidator != null)
            await createValidator.ValidateAndThrowAsync(createDto, cancellationToken);
        return await base.Create(createDto, cancellationToken);
    }

    [HttpPut("{id}")]
    public override async Task<IActionResult> Update(int id, TUpdateDto updateDto, CancellationToken cancellationToken)
    {
        if (updateValidator != null)
            await updateValidator.ValidateAndThrowAsync(updateDto, cancellationToken);
        return await base.Update(id, updateDto, cancellationToken);
    }
}