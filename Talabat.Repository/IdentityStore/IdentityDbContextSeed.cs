using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.IdentityStore
{
    public static class IdentityDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    DisplayName = "Mustafa Mahmoud",
                    Email = "mostafapro87@gmail.com",
                    UserName = "mostafa.mahmoud",
                    PhoneNumber = "01225001973"
                };
                await userManager.CreateAsync(user, "P@ssw0rd");
            }
        }
    }
}
