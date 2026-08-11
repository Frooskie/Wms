using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.Products;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Inventory;

namespace Wms.API.Controllers;

public class ProductController(IProductService productService, IMapper mapper)
    : BaseCrudController<Product, ProductDto, CreateProductRequest, UpdateProductRequest>(productService, mapper)
{
    protected override object GetEntityId(Product entity)
    {
        return entity.Id;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchByName([FromQuery] string name,
        CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsByNameAsync(name, cancellationToken);
        var dtos = Mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dtos);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(string category,
        CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsByCategoryAsync(category, cancellationToken);
        var dtos = Mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dtos);
    }
}