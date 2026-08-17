using AutoMapper;
using FluentValidation;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.WarehouseStructure;

namespace Wms.API.Controllers;

/// <summary>Управление полками.</summary>
public class ShelfController(
    IShelfService shelfService,
    IMapper mapper,
    IValidator<CreateShelfRequest> createShelfValidator,
    IValidator<UpdateShelfRequest> updateShelfValidator)
    : BaseCrudControllerWithValidation<Shelf, ShelfDto, CreateShelfRequest, UpdateShelfRequest>(
        shelfService, mapper, createShelfValidator, updateShelfValidator)
{
    protected override object GetEntityId(Shelf entity)
    {
        return entity.Id;
    }
}