using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.WarehouseStructure;

namespace Wms.API.Controllers;

/// <summary>Управление зонами склада.</summary>
public class ZoneController(
    IZoneService zoneService,
    IMapper mapper,
    IValidator<CreateZoneRequest> createZoneValidator,
    IValidator<UpdateZoneRequest> updateZoneValidator)
    : BaseCrudControllerWithValidation<Zone, ZoneDto, CreateZoneRequest, UpdateZoneRequest>(
        zoneService, mapper, createZoneValidator, updateZoneValidator)
{
    protected override object GetEntityId(Zone entity)
    {
        return entity.Id;
    }

    /// <summary>Получить все зоны с деталями (стеллажи, полки, ячейки).</summary>
    [HttpGet("with-details")]
    public async Task<ActionResult<IEnumerable<ZoneDto>>> GetZonesWithDetails(CancellationToken cancellationToken)
    {
        var zones = await zoneService.GetZonesWithDetailsAsync(cancellationToken);
        var dtos = Mapper.Map<IEnumerable<ZoneDto>>(zones);
        return Ok(dtos);
    }
}