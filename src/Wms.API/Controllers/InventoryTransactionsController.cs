using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FluentValidation;
using Wms.Core.Enums;
using Wms.API.DTOs.Transactions;
using Wms.API.Extensions;
using Wms.Core.DTOs.Common;
using Wms.Core.Interfaces.Services.Audit;

namespace Wms.API.Controllers;

/// <summary>Аудит движений товаров.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class InventoryTransactionsController(
    IInventoryTransactionService transactionService,
    IMapper mapper,
    IValidator<TransactionFilterDto> transactionFilterDtoValidator)
    : ControllerBase
{
    /// <summary>Получить список транзакций с фильтрацией и пагинацией.</summary>
    /// <param name="filter">Параметры фильтрации и пагинации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Страница транзакций.</returns>
    /// <remarks>Доступно для Chief и Manager.</remarks>
    /// <response code="200">Список транзакций успешно получен.</response>
    /// <response code="400">Ошибка валидации фильтра.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<InventoryTransactionResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<ActionResult<PagedResult<InventoryTransactionResponseDto>>> GetTransactions(
        [FromQuery] TransactionFilterDto filter,
        CancellationToken cancellationToken)
    {
        await transactionFilterDtoValidator.ValidateAndThrowAsync(filter, cancellationToken);
        
        // Преобразуем строковый TransactionType в enum, если передан
        TransactionType? transactionType = null;
        if (!string.IsNullOrEmpty(filter.TransactionType) &&
            Enum.TryParse<TransactionType>(filter.TransactionType, true, out var parsed))
        {
            transactionType = parsed;
        }

        var pagedResult = await transactionService.GetPagedFilteredAsync(
            filter.BatchId,
            filter.ProductId,
            filter.UserId,
            transactionType,
            filter.FromDate,
            filter.ToDate,
            filter.PageNumber,
            filter.PageSize,
            cancellationToken);

        var dtoItems = mapper.Map<IEnumerable<InventoryTransactionResponseDto>>(pagedResult.Items);
        var response = new PagedResult<InventoryTransactionResponseDto>(
            dtoItems,
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize);

        return Ok(response);
    }
}