using AutoMapper;
using FluentValidation;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.WarehouseStructure;

namespace Wms.API.Controllers;

public class WarehouseController(
    IWarehouseService service, 
    IMapper mapper,
    IValidator<CreateWarehouseRequest> createWarehouseValidator,
    IValidator<UpdateWarehouseRequest> updateWarehouseValidator)
    : BaseCrudControllerWithValidation<Warehouse, WarehouseDto, CreateWarehouseRequest, UpdateWarehouseRequest>(
        service, mapper, createWarehouseValidator, updateWarehouseValidator)
{
    protected override object GetEntityId(Warehouse entity)
    {
        return entity.Id;
    }
}