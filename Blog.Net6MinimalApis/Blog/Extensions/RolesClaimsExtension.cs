using Blog.Models;
using System.Security.Claims;

namespace Blog.Extensions;

public static class RolesClaimsExtension
{
    public static IEnumerable<Claim> GetClaims(this User user)
    {
        var result = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email) // User.Identity.Name
        };

        result.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x.Slug)));

        return result;
    }
}
