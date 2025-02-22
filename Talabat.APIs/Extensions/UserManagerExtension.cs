using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Extensions
{
    public static class UserManagerExtension
    {
        public static async Task<ApplicationUser> FindUseWithAddressAsync(this UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.Users.Include(U => U.Address).SingleOrDefaultAsync(U => U.Email == email);
            return user!;
        }
    }
}
