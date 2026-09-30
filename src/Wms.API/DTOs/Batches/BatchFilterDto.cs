using System;

namespace Wms.API.DTOs.Batches;

public record BatchFilterDto
{
    public int? ProductId { get; init; }
    public int? CellId { get; init; }
    public DateOnly? ExpiryFrom { get; init; }
    public DateOnly? ExpiryTo { get; init; }

    private int _pageNumber = 1;
    private int _pageSize = 10;
    private const int MaxPageSize = 100;

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            < 1 => 10,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }
}