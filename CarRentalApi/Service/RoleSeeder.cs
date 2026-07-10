using Microsoft.AspNetCore.Identity;

namespace CarRentalApi.Service
{
    public static class RoleSeeder
    {
        private static readonly string[] Roles = ["Admin", "Owner", "Renter"];

        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}