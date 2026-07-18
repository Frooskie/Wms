using AutoMapper;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

public class ShelfController(IShelfService shelfService, IMapper mapper)
    : BaseApiController<Shelf, ShelfDto, CreateShelfRequest, UpdateShelfRequest>(shelfService, mapper)
{
    protected override object GetEntityId(Shelf entity) => entity.Id;
}