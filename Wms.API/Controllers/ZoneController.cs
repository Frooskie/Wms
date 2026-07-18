using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

public class ZoneController(IZoneService zoneService, IMapper mapper)
    : BaseApiController<Zone, ZoneDto, CreateZoneRequest, UpdateZoneRequest>(zoneService, mapper)
{
    protected override object GetEntityId(Zone entity) => entity.Id;
    
    [HttpGet("with-details")]
    public async Task<ActionResult<IEnumerable<ZoneDto>>> GetZonesWithDetails(CancellationToken cancellationToken)
    {
        var zones = await zoneService.GetZonesWithDetailsAsync(cancellationToken);
        var dtos = _mapper.Map<IEnumerable<ZoneDto>>(zones);
        return Ok(dtos);
    }
}