using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Extentions
{
    public static class UserMangerExtentions
    {
        public static async Task<AppUser> FindUserWithAdressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);

            var User = await userManager.Users.Include(U => U.address).FirstOrDefaultAsync(U => U.NormalizedEmail == email.ToUpper());

            return User;
        }
    }
}
