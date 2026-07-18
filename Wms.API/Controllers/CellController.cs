using AutoMapper;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

public class CellController(ICellService cellService, IMapper mapper)
    : BaseApiController<Cell, CellDto, CreateCellRequest, UpdateCellRequest>(cellService, mapper)
{
    protected override object GetEntityId(Cell entity) => entity.Id;
}