using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerAspNetIdentity.Data
{
    public class SeedDefaultUser
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                string[] roles = new string[] { "Owner", "Administrator", "Manager", "Editor", "Buyer", "Business", "Seller", "Subscriber" };

                foreach (string role in roles)
                {
                    using (var roleStore = new RoleStore<IdentityRole>(context))
                    {
                        if (!context.Roles.Any(r => r.Name == role))
                        {
                            await roleStore.CreateAsync(new IdentityRole
                            {
                                Name = role,
                                NormalizedName = role.ToUpper()
                            });
                        }
                    }
                }


                var user = new ApplicationUser
                {
                    //FirstName = "XXXX",
                    //LastName = "XXXX",
                    Email = "admin@lichtrinh.vn",
                    NormalizedEmail = "ADMIN@LICHTRINH.VN",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };


                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(user, "123456");
                    user.PasswordHash = hashed;

                    var userStore = new UserStore<ApplicationUser>(context);
                    var result = userStore.CreateAsync(user);
                }

                AssignRoles(serviceProvider, user.Email, roles);

                await context.SaveChangesAsync();
            }
        }

        public static async void AssignRoles(IServiceProvider serviceProvider, string email, string[] roles)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var _userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>())
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        if (!currentRoles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }
            }
        }
    }
}
