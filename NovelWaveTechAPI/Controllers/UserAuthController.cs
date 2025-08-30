using Application.DTO;
using Application.Permissions;
using Application.Service;
using Application.ServiceInterface;
using Application.ViewModel;
using Azure.Core;
using CaptchaGen;
using CaptchaGen.NetCore;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLaborsCaptcha.Core;
using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        private readonly QRCodeService _qrService;
        private readonly QRCodeWithLogoService _qrWithLogoService;
        //private readonly CryptoHelperService _CryptoHelperService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserAuthController(IServiceInfra userAuthService, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, IWebHostEnvironment env, IConfiguration configuration1, RoleManager<IdentityRole> roleManager)
        {
            _userAuthService = userAuthService;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtKey = configuration["Jwt:Key"];
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
            _JwtExpiry = int.Parse(configuration["Jwt:ExpiryMinutes"]);
            _qrService = new QRCodeService();
            _qrWithLogoService = new QRCodeWithLogoService();
            _env = env;
            _configuration = configuration1;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(int skip, int take)
        {
            var result = await _userAuthService.AuthService.GetUsersIncrementalAsync(skip, take);
            return Ok(result);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string UserIdstr = userId.ToString();
            var result = await _userAuthService.AuthService.GetUsersAsync(UserIdstr);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO == null
                || string.IsNullOrWhiteSpace(registerDTO.Name)
                || string.IsNullOrWhiteSpace(registerDTO.Email)
                || string.IsNullOrWhiteSpace(registerDTO.Password)
                || string.IsNullOrWhiteSpace(registerDTO.CaptchaCode))
            {
                return BadRequest("Invalid registration details");
            }
            var captchaCode = await _userAuthService.AuthService.GetByGenerateCaptchaCodeAsync(registerDTO.CaptchaCode);
            if (captchaCode != null)
            {
                DateTime CreatedDate2 = DateTime.Now;
                if (CreatedDate2 - captchaCode.CreatedDate < TimeSpan.FromMinutes(3))
                {
                    var existingUser = await _userAuthService.AuthService.FindByEmailUserAsync(registerDTO.Email).ConfigureAwait(false);
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
                return BadRequest("Please generate the captcha code and entered the correct captcha code due to the captcha code is expired!");
            }
            return BadRequest("Please generate the captcha code and entered the correct captcha code");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FindByUserName([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO.Name == null
                || string.IsNullOrWhiteSpace(registerDTO.Name))
            {
                return BadRequest("User Name not found");
            }

            var captchaCode = await _userAuthService.AuthService.GetByGenerateCaptchaCodeAsync(registerDTO.CaptchaCode);
            if (captchaCode != null)
            {
                DateTime CreatedDate2 = DateTime.Now;
                if (CreatedDate2 - captchaCode.CreatedDate < TimeSpan.FromMinutes(3))
                {
                    var existingUser = await _userAuthService.AuthService.FindByUserNameAsync(registerDTO.Name).ConfigureAwait(false);
                    if (existingUser == null)
                    {
                        return Conflict("User Name not exists");
                    }

                    return Ok(existingUser);
                }
                return BadRequest("Please generate the captcha code and entered the correct captcha code due to the captcha code is expired!");
            }
            return BadRequest("Please generate the captcha code and entered the correct captcha code");
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

                    //var token = GeneratedJwtToken(result);
                    var token = await GenerateJwtToken(result, _userManager, _configuration);
                    AuthorizationDataDTO authorizationDataDTO = new AuthorizationDataDTO();
                    authorizationDataDTO.token = token;
                    authorizationDataDTO.CreatedDatetime = CreatedDate;
                    await _userAuthService.AuthService.CreateByAuthorizationDataAsync(authorizationDataDTO);
                    return Ok(new { success = true, token });
                }
                return BadRequest("Please generate the captcha code and entered the correct captcha code due to the captcha code is expired!");
            }
            return BadRequest("Please generate the captcha code and entered the correct captcha code");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("User logged out successfully.");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetToken([FromQuery] string token)
        {
            var data = await _userAuthService.AuthService.GetByAuthorizationDataUserIdAsync(token);
            if (data == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                Id = userId,
                Roles = roles
            });
            //return Ok(data);
        }

        [HttpGet("{CaptchaCode}")]
        public async Task<IActionResult> GetCaptcha([FromRoute] string CaptchaCode)
        {
            var data = await _userAuthService.AuthService.GetByGenerateCaptchaCodeAsync(CaptchaCode);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        private string GeneratedJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub ,user.Id),
                new Claim(ClaimTypes.Name ,user.Name),
                new Claim(JwtRegisteredClaimNames.Email , user.Email),
                new Claim("Name" , user.Name),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
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
        [Authorize]
        [HttpGet("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] string Id)
        {
            var data = await _userAuthService.AuthService.Delete(Id);
            return Ok(data);
        }
        [Authorize]
        [HttpPost("{Id}")]
        public async Task<IActionResult> EditUser([FromBody] UserEditDTO userEditDTO, [FromRoute] string Id)
        {
            var data = await _userAuthService.AuthService.Edit(userEditDTO);
            if (data == null) return BadRequest("Update failed");
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
            await _userAuthService.AuthService.DeleteByGenerateCaptchaCodeAsync();
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
        // POST: api/qr/generate
        [HttpPost]
        [Authorize]
        public IActionResult GenerateQRCode([FromBody] QRRequest request)
        {
            DateTime CreatedDatenow = DateTime.Now;
            var loginData = new LoginData
            {
                Email = request.Email,
                Password = request.Password,
                CreatedDate = CreatedDatenow
            };

            var qrBytes = _qrService.GenerateQrCode(loginData);
            return File(qrBytes, "image/png");
        }

        // POST: api/qr/scan
        [HttpPost]
        public async Task<IActionResult> LoginWithQRCode([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is missing.");

            using var stream = file.OpenReadStream();
            try
            {
                var loginData = _qrService.DecodeQrCode(stream);
                if (loginData == null) return BadRequest("Invalid QR Code");
                //if (CreatedDatenow - loginData.CreatedDate > TimeSpan.FromMinutes(2)) return BadRequest("QR Code is expired");
                if (DateTime.UtcNow > loginData.CreatedDate.AddHours(24)) return BadRequest("QR Code is expired");
                var Email = CryptoHelperService.Decrypt(loginData.Email);
                var Password = CryptoHelperService.Decrypt(loginData.Password);
                var result = await _userAuthService.AuthService.FindByEmailUserAsync(Email).ConfigureAwait(false); ;
                if (result == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid username or password" });
                }
                var results = await _userAuthService.AuthService.CheckPasswordSignInAsync(result, Password);
                if (results == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid username or password" });
                }

                //var token = GeneratedJwtToken(result);
                var token = await GenerateJwtToken(result, _userManager, _configuration);
                DateTime CreatedDate = DateTime.Now;
                AuthorizationDataDTO authorizationDataDTO = new AuthorizationDataDTO();
                authorizationDataDTO.token = token;
                authorizationDataDTO.CreatedDatetime = CreatedDate;
                await _userAuthService.AuthService.CreateByAuthorizationDataAsync(authorizationDataDTO);
                return Ok(new { success = true, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/qr/generate-and-save
        [HttpPost]
        [Authorize]
        public IActionResult GenerateAndSaveQRCode([FromBody] QRRequest request)
        {
            DateTime CreatedDatenow = DateTime.Now;
            var loginData = new LoginData
            {
                Email = request.Email,
                Password = request.Password,
                CreatedDate= CreatedDatenow
            };

            string fileName = $"{Guid.NewGuid()}.png";
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "SavedQRCodes");

            // Ensure directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);

            var qrBytes = _qrService.GenerateAndSaveQrCode(loginData, filePath);

            return Ok(new { Message = "QR Code generated and saved.", FilePath = filePath });
        }
        [HttpPost]
        [Authorize]
        public IActionResult GenerateAndSaveQRCodeWithLogo([FromBody] QRRequest request)
        {
            var Email = CryptoHelperService.CryptoHelper.Encrypt(request.Email);
            var Password = CryptoHelperService.CryptoHelper.Encrypt(request.Password);
            DateTime CreatedDatenow = DateTime.Now;
            var loginData = new LoginData
            {
                Email = Email,
                Password = Password,
                CreatedDate = CreatedDatenow
            };
            string fileName = $"{Guid.NewGuid()}.png";
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "SavedQRCodes");

            // Ensure directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);
            //string logofilepath = "/images/logo.png";
            string logofilepath = Path.Combine(_env.WebRootPath, "images", "logo.png");
            var qrBytes = _qrWithLogoService.GenerateQrCodeWithLogoAsync(loginData, filePath);
            if (qrBytes.Result == null) return BadRequest("QRCode Not a generate");
            return Ok(new { Message = "QR Code generated and saved.", FilePath = filePath });
        }
        public async Task<IActionResult> QRScanPost([FromBody] QrDataModel model)
        {
            if (model.QrData == null)
                return BadRequest("No data Found.");
            try
            {
                var parts = model.QrData.Split('|');
                if (parts.Length != 3)
                    throw new Exception("Invalid QR Code format. Expected: Email|Password|CreatedDate");

                if (!DateTime.TryParse(parts[2], out var createdDate))
                    throw new Exception("Invalid date format in QR Code.");

                if (DateTime.UtcNow > createdDate.AddHours(24))
                    throw new Exception("QR Code has expired. It was generated over 24 hours ago.");
                var loginData = new LoginData
                {
                    Email = parts[0],
                    Password = parts[1],
                    CreatedDate = createdDate
                };
                var Email = CryptoHelperService.Decrypt(loginData.Email);
                var Password = CryptoHelperService.Decrypt(loginData.Password);
                var result = await _userAuthService.AuthService.FindByEmailUserAsync(Email).ConfigureAwait(false); ;
                if (result == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid username or password" });
                }
                var results = await _userAuthService.AuthService.CheckPasswordSignInAsync(result, Password);
                if (results == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid username or password" });
                }

                //var token = GeneratedJwtToken(result);
                var token = await GenerateJwtToken(result, _userManager, _configuration);
                DateTime CreatedDate = DateTime.Now;
                AuthorizationDataDTO authorizationDataDTO = new AuthorizationDataDTO();
                authorizationDataDTO.token = token;
                authorizationDataDTO.CreatedDatetime = CreatedDate;
                await _userAuthService.AuthService.CreateByAuthorizationDataAsync(authorizationDataDTO);
                return Ok(new { success = true, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult getusername()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(new { username });
        }
        public async Task<string> GenerateJwtToken(
    ApplicationUser user,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            var assignedPermissions = await _userAuthService.AuthService.GetUserPermissions(user.Id);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

            foreach (var userRole in userRoles.Distinct())
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));

            foreach (var permission in assignedPermissions.Distinct())
                authClaims.Add(new Claim("permission", permission.Permission));

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
            );

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(int.Parse(configuration["Jwt:ExpiryHours"] ?? "3")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Role '{model.Role}' assigned to user {user.UserName}");
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound("User not found");

            if (!await _userManager.IsInRoleAsync(user, model.Role))
                return BadRequest("User does not have this role");

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Role '{model.Role}' removed from user {user.UserName}");
        }

        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }

    }
}
