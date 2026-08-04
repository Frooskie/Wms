using Wms.Core.Entities;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Services.Base;

namespace Wms.Services.ShelfService;

public class ShelfService(IRepository<Shelf> repository) : CrudService<Shelf>(repository), IShelfService
{
}