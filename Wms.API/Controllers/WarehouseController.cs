using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

public class WarehouseController(IWarehouseService service, IMapper mapper)
    : BaseApiController<Warehouse, WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest>(service, mapper)
{
    protected override object GetEntityId(Warehouse entity) => entity.Id;
}