using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Wms.Core.Enums;
using Wms.Core.Interfaces.Services;
using Wms.API.DTOs.Transactions;
using Wms.Core.Interfaces.Services.Audit;

namespace Wms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryTransactionsController(IInventoryTransactionService transactionService, IMapper mapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryTransactionResponseDto>>> GetTransactions(
        [FromQuery] TransactionFilterDto filter,
        CancellationToken cancellationToken)
    {
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