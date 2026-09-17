using AutoMapper;
using Application.Dto.User;
using Domain.Entities;
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
            if (user == null) return NotFound();
            return _mapper.Map<UserProfileDto>(user);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileModel request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("User not found");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
            {
                var firstName = string.IsNullOrEmpty(request.FirstName) ? user.FirstName : request.FirstName;
                var lastName = string.IsNullOrEmpty(request.LastName) ? user.LastName : request.LastName;
                user.UpdateName(firstName, lastName);
            }

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(_mapper.Map<UserProfileDto>(user));
        }

        [HttpPost("add-driver-license")]
        public async Task<ActionResult<UserProfileDto>> AddDriverLicense([FromBody] AddDrivingLicenceRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (request.ExpiryDate < DateTime.Today)
                return BadRequest("Driver license has expired");

            user.AddDriverLicense(request.LicenseNumber, request.ExpiryDate);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(_mapper.Map<UserProfileDto>(user));
        }
    }
}
