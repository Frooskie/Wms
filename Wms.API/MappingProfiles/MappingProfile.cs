using AutoMapper;
using Wms.Core.Enums;
using Wms.API.DTOs;
using Wms.Core.Entities;

namespace Wms.API.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseRequest, Warehouse>();
        CreateMap<UpdateWarehouseRequest, Warehouse>();
        
        CreateMap<Zone, ZoneDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        CreateMap<CreateZoneRequest, Zone>();
        CreateMap<UpdateZoneRequest, Zone>();
        
        CreateMap<Rack, RackDto>();
        CreateMap<CreateRackRequest, Rack>();
        CreateMap<UpdateRackRequest, Rack>();
        
        CreateMap<Shelf, ShelfDto>();
        CreateMap<CreateShelfRequest, Shelf>();
        CreateMap<UpdateShelfRequest, Shelf>();

        CreateMap<Cell, CellDto>();
        CreateMap<CreateCellRequest, Cell>();
        CreateMap<UpdateCellRequest, Cell>();

        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();
        
        CreateMap<CreateBatchRequest, Batch>();
        CreateMap<Batch, BatchDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.CellCode, opt => opt.MapFrom(src => src.Cell.Code))
            .ForMember(dest => dest.ShelfNumber, opt => opt.MapFrom(src => src.Cell.Shelf.Number.ToString()))
            .ForMember(dest => dest.RackCode, opt => opt.MapFrom(src => src.Cell.Shelf.Rack.Code))
            .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Cell.Shelf.Rack.Zone.Name))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Quantity - src.ReservedQuantity));
        CreateMap<UpdateBatchRequest, Batch>()
            .ForMember(dest => dest.CellId, opt => opt.Ignore()) // для изменения ячейки отдельный метод
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Quantity, opt => opt.Ignore())
            .ForMember(dest => dest.ReservedQuantity, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedDate, opt => opt.Ignore());
    }
}