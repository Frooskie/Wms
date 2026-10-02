using AutoMapper;
using Wms.API.DTOs.Batches;
using Wms.API.DTOs.Products;
using Wms.API.DTOs.Receipts;
using Wms.API.DTOs.Supply;
using Wms.API.DTOs.Transactions;
using Wms.API.DTOs.WarehouseStructure;
using Wms.Core.Entities;

namespace Wms.API.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseRequest, Warehouse>();
        CreateMap<UpdateWarehouseRequest, Warehouse>();

        CreateMap<Zone, ZoneDto>();
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

        CreateMap<Receipt, ReceiptDto>()
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lines));

        CreateMap<ReceiptLine, ReceiptLineDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<SupplyRequest, SupplyRequestDto>()
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lines));

        CreateMap<SupplyRequestLine, SupplyRequestLineDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        
        CreateMap<SupplyOrder, SupplyOrderDto>()
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lines))
            .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations));

        CreateMap<SupplyOrderLine, SupplyOrderLineDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<Reservation, ReservationDto>();
        
        CreateMap<InventoryTransaction, InventoryTransactionResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Batch.Product.Name))
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.ToString()))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.OldCellCode, opt => opt.MapFrom(src => src.OldCell != null ? src.OldCell.Code : null))
            .ForMember(dest => dest.NewCellCode, opt => opt.MapFrom(src => src.NewCell != null ? src.NewCell.Code : null));
    }
}