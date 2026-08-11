using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Warehouse;
using Wms.Services.Base;

namespace Wms.Services.Warehouse;

public class WarehouseService(IRepository<Core.Entities.Warehouse> repository) : CrudService<Core.Entities.Warehouse>(repository), IWarehouseService
{
}