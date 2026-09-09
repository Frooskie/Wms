using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.WarehouseStructure;

namespace Wms.API.Controllers;

/// <summary>Управление стеллажами.</summary>
public class RackController(
    IRackService rackService,
    IMapper mapper,
    IValidator<CreateRackRequest> createRackValidator,
    IValidator<UpdateRackRequest> updateRackValidator)
    : BaseCrudControllerWithValidation<Rack, RackDto, CreateRackRequest, UpdateRackRequest>(
        rackService, mapper, createRackValidator, updateRackValidator)
{
    protected override object GetEntityId(Rack entity)
    {
        return entity.Id;
    }

    /// <summary>Получить стеллажи с вложенными полками и ячейками.</summary>
    /// <response code="200">Список стеллажей с полками и ячейками.</response>
    [HttpGet("with-shelves")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RackDto>))]
    public async Task<ActionResult<IEnumerable<RackDto>>> GetRacksWithShelvesAndCells(
        CancellationToken cancellationToken)
    {
        var racks = await rackService.GetRacksWithShelvesAndCellsAsync(cancellationToken);
        var dtos = Mapper.Map<IEnumerable<RackDto>>(racks);
        return Ok(dtos);
    }
}