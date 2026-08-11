using Wms.Core.Entities;

namespace Wms.Core.Interfaces.Services.Auth;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}