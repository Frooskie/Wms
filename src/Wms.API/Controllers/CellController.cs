using AutoMapper;
using FluentValidation;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.WarehouseStructure;

namespace Wms.API.Controllers;

/// <summary>Управление ячейками.</summary>
public class CellController(
    ICellService cellService,
    IMapper mapper,
    IValidator<CreateCellRequest> createCellValidator,
    IValidator<UpdateCellRequest> updateCellValidator)
    : BaseCrudControllerWithValidation<Cell, CellDto, CreateCellRequest, UpdateCellRequest>(
        cellService, mapper, createCellValidator, updateCellValidator)
{
    protected override object GetEntityId(Cell entity)
    {
        return entity.Id;
    }
}