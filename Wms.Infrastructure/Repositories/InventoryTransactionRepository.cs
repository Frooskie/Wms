using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Infrastructure.Data;

namespace Wms.Infrastructure.Repositories;

public class InventoryTransactionRepository(ApplicationDbContext context)
    : Repository<InventoryTransaction>(context), IInventoryTransactionRepository;