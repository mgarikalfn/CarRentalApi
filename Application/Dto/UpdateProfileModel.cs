using System.ComponentModel.DataAnnotations;

namespace Application.Dto.User
{
    public class UpdateProfileModel
    {
        

        public string?  Email { get; set; }
        
        public string? FirstName { get; set; }
        
        public string ?LastName { get; set; }
    }
}
