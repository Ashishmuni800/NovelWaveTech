using Application.DTO;
using Application.ServiceInterface;
using CaptchaGen;
using CaptchaGen.NetCore;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SixLaborsCaptcha.Core;
using SixLabors.ImageSharp;
using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SixLabors.ImageSharp.Formats.Webp;
namespace NovelWaveTechAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IServiceInfra _userAuthService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _jwtKey;
        private readonly string? _jwtIssuer;
        private readonly string? _jwtAudience;
        private readonly int _JwtExpiry;
        public UserAuthController(IServiceInfra userAuthService, UserManager<ApplicationUser> userManager,
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
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO == null
                || string.IsNullOrWhiteSpace(registerDTO.Name)
                || string.IsNullOrWhiteSpace(registerDTO.Email)
                || string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                return BadRequest("Invalid registration details");
            }

            var existingUser = await _userAuthService.AuthService.FindByEmailAsync(registerDTO.Email).ConfigureAwait(false);
            if (existingUser != null)
            {
                return Conflict("Email already exists");
            }

            var result = await _userAuthService.AuthService.RegisterAsync(registerDTO).ConfigureAwait(false);
            if (result == null)
            {
                return BadRequest("User creation failed");
            }

            return Ok("User created successfully");
        }

        [HttpPost]
        public async Task<IActionResult> FindByUserName([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO.Name == null
                || string.IsNullOrWhiteSpace(registerDTO.Name))
            {
                return BadRequest("User Name not found");
            }

            var existingUser = await _userAuthService.AuthService.FindByUserNameAsync(registerDTO.Name).ConfigureAwait(false);
            if (existingUser == null)
            {
                return Conflict("User Name not exists");
            }

            return Ok(existingUser);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO == null
                || string.IsNullOrWhiteSpace(changePasswordDTO.UserName)
                || string.IsNullOrWhiteSpace(changePasswordDTO.Password)
                || string.IsNullOrWhiteSpace(changePasswordDTO.CaptchaCode))
            {
                return BadRequest("User Name not found");
            }
            var captchaCode = await _userAuthService.AuthService.GetByGenerateCaptchaCodeAsync(changePasswordDTO.CaptchaCode);
            if (captchaCode != null)
            {
                DateTime CreatedDate2 = DateTime.Now;
                if (CreatedDate2 - captchaCode.CreatedDate < TimeSpan.FromMinutes(3))
                {
                    if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmNewPassword)
                    {
                        return BadRequest("New Password and Confirm New Password does not matched");
                    }
                    if (changePasswordDTO.Password == changePasswordDTO.NewPassword || changePasswordDTO.Password == changePasswordDTO.ConfirmNewPassword)
                    {
                        return BadRequest("New Password and Password is matched. Please try again with different password");
                    }
                    var data = await _userAuthService.AuthService.GetByPasswordChangeHistoryAsync(changePasswordDTO.NewPassword);
                    if (data.Count() < 5)
                    {
                        var existingUser = await _userAuthService.AuthService.ChangePasswordAsync(changePasswordDTO).ConfigureAwait(false);
                        if (existingUser == true)
                        {
                            DateTime CreatedDate = DateTime.UtcNow;
                            var PasswordChangeHistoryDTO = new PasswordChangeHistoryDTO
                            {
                                UserPassword = changePasswordDTO.NewPassword,
                                OldPassword = changePasswordDTO.Password,
                                CreatedDate = CreatedDate,
                            };
                            await _userAuthService.AuthService.CreateByPasswordChangeHistoryAsync(PasswordChangeHistoryDTO);
                            return Ok("Password is sucessfully channged.");
                        }
                        return BadRequest("Password does not channged.");
                    }
                    return BadRequest("The password can not be same as any of the last 5 passwords.");
                }
                return BadRequest("Please generate the captcha code and entered the correct captcha code due to the captcha code is expired!");
            }
            return BadRequest("Please generate the captcha code and entered the correct captcha code");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var captchaCode = await _userAuthService.AuthService.GetByGenerateCaptchaCodeAsync(loginDTO.CaptchaCode);
            if (captchaCode != null)
            {
                DateTime CreatedDate = DateTime.Now;
                if(CreatedDate-captchaCode.CreatedDate<TimeSpan.FromMinutes(3))
                {
                    var result = await _userAuthService.AuthService.FindByEmailUserAsync(loginDTO.Email).ConfigureAwait(false); ;
                    if (result == null)
                    {
                        return Unauthorized(new { success = false, message = "Invalid username or password" });
                    }
                    var results = await _userAuthService.AuthService.CheckPasswordSignInAsync(result, loginDTO.Password);
                    if (results == null)
                    {
                        return Unauthorized(new { success = false, message = "Invalid username or password" });
                    }

                    var token = GeneratedJwtToken(result);
                    return Ok(new { success = true, token });
                }
                return BadRequest("Please generate the captcha code and entered the correct captcha code due to the captcha code is expired!");
            }
            return BadRequest("Please generate the captcha code and entered the correct captcha code");
        }

        [HttpPost]
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
        [Authorize]
        [HttpGet("{UserPassword}")]
        public async Task<IActionResult> GetByPasswordChangeHistory([FromRoute] string UserPassword)
        {
            var data= await _userAuthService.AuthService.GetByPasswordChangeHistoryAsync(UserPassword);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GenerateCaptchaForApi()
        {
            var code = await _userAuthService.GenerateRandomCaptchaCode.GenerateRandomCode(6);
            DateTime CreatedDate = DateTime.Now;
            var GenerateCaptchaCodeDTO = new GenerateCaptchaCodeDTO
            {
                CaptchaCode = code,
                CreatedDate = CreatedDate,
            };
            await _userAuthService.AuthService.CreateByGenerateCaptchaCodeAsync(GenerateCaptchaCodeDTO);
            return Ok(code);
        }
        [HttpGet]
        public async Task<IActionResult> GenerateCaptcha()
        {
            var code = await _userAuthService.GenerateRandomCaptchaCode.GenerateRandomCode(6);
            SixLaborsCaptchaModule sixLaborsCaptcha = new SixLaborsCaptchaModule(new SixLaborsCaptchaOptions
            {
                Width = 200,
                Height = 40,
                FontSize = 25,
                NoiseRate = (ushort)0.4f,
                TextColor = new SixLabors.ImageSharp.Color[]
                {
                    SixLabors.ImageSharp.Color.Black,
                },
                BackgroundColor = new SixLabors.ImageSharp.Color[]
                {
                    SixLabors.ImageSharp.Color.LightGray,
                },
                FontFamilies = new[] { "Arial", "Times New Roman" }
            });
            byte[] imageStream = sixLaborsCaptcha.Generate(code);
            var generateCaptchaCodeDTO = new GenerateCaptchaCodeDTO
            {
                CaptchaCode = code,
                CreatedDate = DateTime.Now,
            };
            await _userAuthService.AuthService.CreateByGenerateCaptchaCodeAsync(generateCaptchaCodeDTO);
            await _userAuthService.AuthService.DeleteByGenerateCaptchaCodeAsync();
            return File(imageStream, "image/png");
        }
    }
 }
