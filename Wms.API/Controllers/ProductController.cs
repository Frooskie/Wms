using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services;

namespace Wms.API.Controllers;

public class ProductController(IProductService productService, IMapper mapper)
    : BaseApiController<Product, ProductDto, CreateProductRequest, UpdateProductRequest>(productService, mapper)
{
    protected override object GetEntityId(Product entity) => entity.Id;
    
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsByNameAsync(name, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dtos);
    }
    
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(string category, CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsByCategoryAsync(category, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dtos);
    }
}