using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

public class WarehouseController(IWarehouseService service, IMapper mapper)
    : BaseCrudController<Warehouse, WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest>(service, mapper)
{
    protected override object GetEntityId(Warehouse entity) => entity.Id;
}