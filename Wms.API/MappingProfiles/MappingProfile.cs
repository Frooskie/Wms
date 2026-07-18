using AutoMapper;
using Wms.Core.Enums;
using Wms.API.DTOs;
using Wms.Core.Entities;

namespace Wms.API.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Warehouse
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseRequest, Warehouse>();
        CreateMap<UpdateWarehouseRequest, Warehouse>();

        // Zone
        CreateMap<Zone, ZoneDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        CreateMap<CreateZoneRequest, Zone>();
        CreateMap<UpdateZoneRequest, Zone>();

        // Rack
        CreateMap<Rack, RackDto>();
        CreateMap<CreateRackRequest, Rack>();
        CreateMap<UpdateRackRequest, Rack>();

        // Shelf
        CreateMap<Shelf, ShelfDto>();
        CreateMap<CreateShelfRequest, Shelf>();
        CreateMap<UpdateShelfRequest, Shelf>();

        // Cell
        CreateMap<Cell, CellDto>();
        CreateMap<CreateCellRequest, Cell>();
        CreateMap<UpdateCellRequest, Cell>();

        // Product
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();
    }
}