using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

public class RackController(IRackService rackService, IMapper mapper)
    : BaseApiController<Rack, RackDto, CreateRackRequest, UpdateRackRequest>(rackService, mapper)
{
    protected override object GetEntityId(Rack entity) => entity.Id;
    
    [HttpGet("with-shelves")]
    public async Task<ActionResult<IEnumerable<RackDto>>> GetRacksWithShelvesAndCells(CancellationToken cancellationToken)
    {
        var racks = await rackService.GetRacksWithShelvesAndCellsAsync(cancellationToken);
        var dtos = _mapper.Map<IEnumerable<RackDto>>(racks);
        return Ok(dtos);
    }
}