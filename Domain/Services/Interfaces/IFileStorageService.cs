using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Domain.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveVehicleImageAsync(IFormFile imageFile);
        bool DeleteVehicleImageAsync(string imageUrl);
    }
}
