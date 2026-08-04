using Wms.Core.Entities;

namespace Wms.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}