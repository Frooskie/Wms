using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.WarehouseService;

public class WarehouseService(IRepository<Warehouse> repository) : BaseService<Warehouse>(repository), IWarehouseService
{

}