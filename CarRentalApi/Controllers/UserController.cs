using AutoMapper;
using CarRentalApi.Dto;
using CarRentalApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        private readonly IMapper _mapper;
        public UserController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetCurrentUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserProfileDto>(user);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileModel request)
        {
       
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                user.FirstName = request.FirstName;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                user.LastName = request.LastName;
            }

            /* if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
             {
                 // Check if email is already taken
                 var existingUser = await _userManager.FindByEmailAsync(request.Email);
                 if (existingUser != null && existingUser.Id != user.Id)
                 {
                     return BadRequest("Email is already in use by another account");
                 }

                 // Update email and reset email confirmation status
                 user.Email = request.Email;
                 user.NormalizedEmail = _userManager.NormalizeEmail(request.Email);
                 user.EmailConfirmed = false;

                 // Consider sending verification email here
             }
            */

            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }
            // Save changes
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            // Return updated profile
            return Ok(_mapper.Map<UserProfileDto>(user));
        }

        [HttpPost("add-driver-license")]
        public async Task<ActionResult<UserProfileDto>> AddDriverLicense(
        [FromBody] AddDrivingLicenceRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Validate expiry date
            if (request.ExpiryDate < DateTime.Today)
            {
                return BadRequest("Driver license has expired");
            }

            
            user.DriverLicenseNumber = request.LicenseNumber;
            user.DriverLicenseExpiryDate = request.ExpiryDate;
            user.IsDriverLicenseVerified = false; // Reset verification status

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(_mapper.Map<UserProfileDto>(user));
        }


    }
}
