using System.ComponentModel.DataAnnotations;

namespace CarRentalApi.Dto
{
    public class UpdateProfileModel
    {
        

        public string?  Email { get; set; }
        
        public string? FirstName { get; set; }
        
        public string ?LastName { get; set; }
    }
}
