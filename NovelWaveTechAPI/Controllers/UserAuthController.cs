using Application.DTO;
using Application.ServiceInterface;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NovelWaveTechAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _jwtKey;
        private readonly string? _jwtIssuer;
        private readonly string? _jwtAudience;
        private readonly int _JwtExpiry;
        public UserAuthController(IUserAuthService userAuthService, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userAuthService = userAuthService;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtKey = configuration["Jwt:Key"];
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
            _JwtExpiry = int.Parse(configuration["Jwt:ExpiryMinutes"]);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO == null
                || string.IsNullOrWhiteSpace(registerDTO.Name)
                || string.IsNullOrWhiteSpace(registerDTO.Email)
                || string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                return BadRequest("Invalid registration details");
            }

            var existingUser = await _userAuthService.FindByEmailAsync(registerDTO.Email).ConfigureAwait(false);
            if (existingUser != null)
            {
                return Conflict("Email already exists");
            }

            var result = await _userAuthService.RegisterAsync(registerDTO).ConfigureAwait(false);
            if (result == null)
            {
                return BadRequest("User creation failed");
            }

            return Ok("User created successfully");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _userAuthService.FindByEmailUserAsync(loginDTO.Email).ConfigureAwait(false); ;
            if (result == null)
            {
                return Unauthorized(new { success = false, message = "Invalid username or password" });
            }
            var results = await _userAuthService.CheckPasswordSignInAsync(result, loginDTO.Password);
            if (results ==null)
            {
                return Unauthorized(new { success = false, message = "Invalid username or password" });
            }

            var token = GeneratedJwtToken(result);
            return Ok(new { success = true, token });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("User logged out successfully.");
        }
        private string GeneratedJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub ,user.Id),
                new Claim(JwtRegisteredClaimNames.Email , user.Email),
                new Claim("Name" , user.Name),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                //issuer: _jwtIssuer,
                //audience: _jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_JwtExpiry),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
