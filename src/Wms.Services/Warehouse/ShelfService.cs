using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Warehouse;
using Wms.Services.Base;

namespace Wms.Services.Warehouse;

public class ShelfService(IRepository<Shelf> repository) : CrudService<Shelf>(repository), IShelfService
{
}