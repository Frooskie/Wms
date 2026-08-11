using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Warehouse;

namespace Wms.API.Controllers;

public class RackController(IRackService rackService, IMapper mapper)
    : BaseCrudController<Rack, RackDto, CreateRackRequest, UpdateRackRequest>(rackService, mapper)
{
    protected override object GetEntityId(Rack entity)
    {
        return entity.Id;
    }

    [HttpGet("with-shelves")]
    public async Task<ActionResult<IEnumerable<RackDto>>> GetRacksWithShelvesAndCells(
        CancellationToken cancellationToken)
    {
        var racks = await rackService.GetRacksWithShelvesAndCellsAsync(cancellationToken);
        var dtos = Mapper.Map<IEnumerable<RackDto>>(racks);
        return Ok(dtos);
    }
}