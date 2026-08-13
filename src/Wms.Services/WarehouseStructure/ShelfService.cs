using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services.WarehouseStructure;
using Wms.Services.Base;

namespace Wms.Services.WarehouseStructure;

public class ShelfService(IRepository<Shelf> repository) : CrudService<Shelf>(repository), IShelfService
{
}