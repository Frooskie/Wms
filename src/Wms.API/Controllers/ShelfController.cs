using AutoMapper;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Warehouse;

namespace Wms.API.Controllers;

public class ShelfController(IShelfService shelfService, IMapper mapper)
    : BaseCrudController<Shelf, ShelfDto, CreateShelfRequest, UpdateShelfRequest>(shelfService, mapper)
{
    protected override object GetEntityId(Shelf entity)
    {
        return entity.Id;
    }
}