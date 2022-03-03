using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUser(UserManager<ApplicationUser> userManager,
                                           RoleManager<ApplicationRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<ApplicationUser>>(userData);
            if (users == null) return;

            var roles = new List<ApplicationRole>
            { 
                new ApplicationRole{Name = "Member"},
                new ApplicationRole{Name = "Admin"},
                new ApplicationRole{Name = "Moderator"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);    
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();

                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new ApplicationUser 
            { UserName = "admin" };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
           

        }
    }
}
