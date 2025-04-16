using CarRentalApi.Entities;

namespace CarRentalApi.Service
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(ApplicationUser applicationUser);
    }
}
