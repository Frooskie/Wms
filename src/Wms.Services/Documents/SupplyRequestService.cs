using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Enums;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Documents;

namespace Wms.Services.Documents;

public class SupplyRequestService(
    ISupplyRequestRepository repository,
    IRepository<SupplyRequestLine> lineRepository,
    IProductRepository productRepository)
    : ISupplyRequestService
{
    public async Task<SupplyRequest> CreateSupplyRequestAsync(SupplyRequest request, List<SupplyRequestLine> lines,
        CancellationToken cancellationToken = default)
    {
        foreach (var line in lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(ErrorMessages.Product.NotFoundFormat(line.ProductId));
        }

        request.Lines = lines;

        await repository.AddAsync(request, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<SupplyRequest?> GetByIdWithLinesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await repository.GetSupplyRequestWithLinesAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<SupplyRequest>> GetAllWithLinesAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetSupplyRequestsWithLinesAsync(cancellationToken);
    }

    public async Task SubmitAsync(int id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);

        if (request == null)
            throw new NotFoundException(ErrorMessages.SupplyRequest.NotFoundFormat(id));
        if (request.CreatedBy != currentUserId)
            throw new ForbiddenAccessException(ErrorMessages.SupplyRequest.OnlyCreatorCanSubmit);
        if (request.Status != SupplyRequestStatus.Draft)
            throw new BusinessRuleException(ErrorMessages.SupplyRequest.OnlyDraftCanBeSubmitted, "REQUEST_NOT_DRAFT");

        request.Status = SupplyRequestStatus.Submitted;
        request.SubmittedAt = DateTime.UtcNow;

        repository.Update(request);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);

        if (request == null)
            throw new NotFoundException(ErrorMessages.SupplyRequest.NotFoundFormat(id));
        if (request.Status != SupplyRequestStatus.Submitted)
            throw new BusinessRuleException(ErrorMessages.SupplyRequest.OnlySubmittedCanBeApproved, "REQUEST_NOT_SUBMITTED");

        request.Status = SupplyRequestStatus.Approved;
        request.ApprovedAt = DateTime.UtcNow;
        
        repository.Update(request);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(int id, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetByIdAsync(id, cancellationToken);
        
        if (request == null)
            throw new NotFoundException(ErrorMessages.SupplyRequest.NotFoundFormat(id));
        if (request.Status != SupplyRequestStatus.Submitted && request.Status != SupplyRequestStatus.Draft)
            throw new BusinessRuleException(ErrorMessages.SupplyRequest.OnlyDraftOrSubmittedCanBeRejected, "REQUEST_NOT_REJECTABLE");
        
        request.Status = SupplyRequestStatus.Rejected;
        request.RejectedAt = DateTime.UtcNow;
        
        repository.Update(request);
        await repository.SaveChangesAsync(cancellationToken);
    }
}