using AutoMapper;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Warehouse;

namespace Wms.API.Controllers;

public class CellController(ICellService cellService, IMapper mapper)
    : BaseCrudController<Cell, CellDto, CreateCellRequest, UpdateCellRequest>(cellService, mapper)
{
    protected override object GetEntityId(Cell entity)
    {
        return entity.Id;
    }
}