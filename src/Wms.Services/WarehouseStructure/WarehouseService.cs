using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.WarehouseStructure;
using Wms.Services.Base;

namespace Wms.Services.WarehouseStructure;

public class WarehouseService(IRepository<Core.Entities.Warehouse> repository) : CrudService<Core.Entities.Warehouse>(repository), IWarehouseService
{
}