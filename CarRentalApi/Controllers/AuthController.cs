using System.Security.Claims;
using CarRentalApi.Dto;
using CarRentalApi.Entities;
using CarRentalApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,IConfiguration configuration,ITokenService tokenService)
        {
         _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, registerDto.ChooseRole);
                // Add default claim for new users
                await _userManager.AddClaimAsync(user, new Claim("UserType", "Regular"));


                // Add driver license claim if provided
                if (!string.IsNullOrEmpty(registerDto.DriverLicenseNumber))
                {
                    await _userManager.AddClaimAsync(user,
                        new Claim("DriverLicense", registerDto.DriverLicenseNumber));
                }

                return Ok(new { Message = "User registered successfully" });


            }
            return BadRequest(result.Errors);
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find user by email (async)
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid login attempt" });
            }


            // Attempt login
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, // Use UserName instead of Email
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var token = await _tokenService.GenerateToken(user);
                return Ok(new
                {
                    Message = "User logged in successfully",
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user) // Include user roles
                });
            }

            return Unauthorized(new { Message = "Invalid login attempt" });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
