using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FluentValidation;
using Wms.Core.Enums;
using Wms.API.DTOs.Transactions;
using Wms.API.Extensions;
using Wms.Core.Interfaces.Services.Audit;

namespace Wms.API.Controllers;

/// <summary>Аудит движений товаров.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryTransactionsController(
    IInventoryTransactionService transactionService,
    IMapper mapper,
    IValidator<TransactionFilterDto> transactionFilterDtoValidator)
    : ControllerBase
{
    /// <summary>Получить список транзакций с фильтрацией.</summary>
    /// <returns>Список транзакций.</returns>
    /// <remarks>
    /// Доступно для Chief и Manager.
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryTransactionResponseDto>>> GetTransactions(
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

        var transactions = await transactionService.GetFilteredAsync(
            filter.BatchId,
            filter.ProductId,
            filter.UserId,
            transactionType,
            filter.FromDate,
            filter.ToDate,
            cancellationToken);

        var dtos = mapper.Map<IEnumerable<InventoryTransactionResponseDto>>(transactions);
        return Ok(dtos);
    }
}