using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wms.API.Controllers.Base;
using Wms.API.DTOs.Products;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Inventory;

namespace Wms.API.Controllers;

/// <summary>Управление справочником товаров.</summary>
public class ProductController(
    IProductService productService,
    IMapper mapper,
    IValidator<CreateProductRequest> createProductValidator,
    IValidator<UpdateProductRequest> updateProductValidator)
    : BaseCrudControllerWithValidation<Product, ProductDto, CreateProductRequest, UpdateProductRequest>(
        productService, mapper, createProductValidator, updateProductValidator)
{
    protected override object GetEntityId(Product entity)
    {
        return entity.Id;
    }

    /// <summary>Поиск товаров по названию (частичное совпадение).</summary>
    /// <param name="name">Название товара.</param>
    /// <response code="200">Список найденных товаров.</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDto>))]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchByName([FromQuery] string name,
        CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsByNameAsync(name, cancellationToken);
        var dtos = Mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dtos);
    }

    /// <summary>Получить товары по категории.</summary>
    /// <param name="category">Категория.</param>
    /// <response code="200">Список товаров в категории.</response>
    [HttpGet("category/{category}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDto>))]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(string category,
        CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsByCategoryAsync(category, cancellationToken);
        var dtos = Mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(dtos);
    }
}