using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;

namespace Wms.Services.Supply;

public class SupplyRequestService(
    ISupplyRequestRepository repository,
    IRepository<SupplyRequestLine> lineRepository,
    IProductRepository productRepository)
    : ISupplyRequestService
{
    public async Task<SupplyRequest> CreateSupplyRequestAsync(SupplyRequest request, List<SupplyRequestLine> lines, CancellationToken cancellationToken = default)
    {
        foreach (var line in lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), line.ProductId);
        }
        request.Lines = lines;
        await repository.AddAsync(request, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<SupplyRequest?> GetByIdWithLinesAsync(int id, CancellationToken cancellationToken = default)
        => await repository.GetSupplyRequestWithLinesAsync(id, cancellationToken);

    public async Task<IEnumerable<SupplyRequest>> GetAllWithLinesAsync(CancellationToken cancellationToken = default)
        => await repository.GetSupplyRequestsWithLinesAsync(cancellationToken);

    public async Task SubmitAsync(int id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            throw new NotFoundException(nameof(SupplyRequest), id);
        if (request.CreatedBy != currentUserId)
            throw new ForbiddenAccessException("Only the creator can submit the request.");
        if (request.Status != SupplyRequestStatus.Draft)
            throw new BusinessRuleException("Only draft requests can be submitted.");
        request.Status = SupplyRequestStatus.Submitted;
        request.SubmittedAt = DateTime.UtcNow;
        repository.Update(request);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            throw new NotFoundException(nameof(SupplyRequest), id);
        if (request.Status != SupplyRequestStatus.Submitted)
            throw new BusinessRuleException("Only submitted requests can be approved.");
        request.Status = SupplyRequestStatus.Approved;
        request.ApprovedAt = DateTime.UtcNow;
        repository.Update(request);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            throw new NotFoundException(nameof(SupplyRequest), id);
        if (request.Status != SupplyRequestStatus.Submitted && request.Status != SupplyRequestStatus.Draft)
            throw new BusinessRuleException("Only draft or submitted requests can be rejected.");
        request.Status = SupplyRequestStatus.Rejected;
        request.RejectedAt = DateTime.UtcNow;
        repository.Update(request);
        await repository.SaveChangesAsync(cancellationToken);
    }
}