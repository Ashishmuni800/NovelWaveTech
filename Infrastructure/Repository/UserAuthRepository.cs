using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _jwtKey;
        private readonly string? _jwtIssuer;
        private readonly string? _jwtAudience;
        private readonly int _JwtExpiry;
        private readonly ApplicationDbContext _dbContext;

        public UserAuthRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, ApplicationDbContext dbContext)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _jwtKey = configuration["Jwt:Key"];
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
            _JwtExpiry = int.Parse(configuration["Jwt:ExpiryMinutes"]);
            _dbContext = dbContext;

        }

        public async Task<AuthorizationData> CreateByAuthorizationDataAsync(AuthorizationData authorizationData)
        {
            AuthorizationData authorization = new AuthorizationData();
            Guid Id = Guid.NewGuid();
            authorization.Id = Id.ToString();
            authorization.token = authorizationData.token;
            authorization.CreatedDatetime = authorizationData.CreatedDatetime;
            await _dbContext.AuthorizationData.AddAsync(authorization);
            await _dbContext.SaveChangesAsync();

            return authorization; // ← returning the saved entity makes more sense
        }

        public async Task<GenerateCaptchaCode> CreateByGenerateCaptchaCodeAsync(GenerateCaptchaCode captchaCode)
        {
            GenerateCaptchaCode generateCaptcha = new GenerateCaptchaCode();
            Guid Id = Guid.NewGuid();
            generateCaptcha.Id = Id.ToString();
            generateCaptcha.CaptchaCode = captchaCode.CaptchaCode;
            generateCaptcha.CreatedDate = captchaCode.CreatedDate;
            await _dbContext.GenerateCaptchaCode.AddAsync(generateCaptcha);
            await _dbContext.SaveChangesAsync();

            return captchaCode; // ← returning the saved entity makes more sense
        }

        public async Task<PasswordChangeHistory> CreateByPasswordChangeHistoryAsync(PasswordChangeHistory passwordChangeHistoryModel)
        {
            if (passwordChangeHistoryModel == null)
                throw new ArgumentNullException(nameof(passwordChangeHistoryModel));

            var passwordHistory = new PasswordChangeHistory
            {
                Id = passwordChangeHistoryModel.Id,
                UserPassword = passwordChangeHistoryModel.UserPassword,
                OldPassword = passwordChangeHistoryModel.OldPassword,
                CreatedDate = passwordChangeHistoryModel.CreatedDate,
            };

            await _dbContext.PasswordChangeHistory.AddAsync(passwordHistory);
            await _dbContext.SaveChangesAsync();

            return passwordHistory; // ← returning the saved entity makes more sense
        }

        public async Task<bool> DeleteByAuthorizationDataAsync()
        {
            var ExpireDataTime = DateTime.Now.AddMinutes(-5);
            var OldData = _dbContext.AuthorizationData.Where(op => op.CreatedDatetime < ExpireDataTime);
            if (OldData.Count() > 0)
            {
                _dbContext.AuthorizationData.RemoveRange(OldData);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteByGenerateCaptchaCodeAsync()
        {
            var ExpireDataTime = DateTime.Now.AddMinutes(-5);
            var OldData = _dbContext.GenerateCaptchaCode.Where(op => op.CreatedDate < ExpireDataTime);
            if(OldData.Count()>0)
            {
                _dbContext.GenerateCaptchaCode.RemoveRange(OldData);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<RegisterModel> FindByEmailAsync(string Email)
        {
            RegisterModel register=new RegisterModel();
            register.Email=Email;
            var result= await _userManager.FindByEmailAsync(Email);
            if (result == null)
                return null;
            else
            {
                return register;
            }
        }

        public async Task<RegisterModel> FindByUserNameAsync(string Name)
        {
            RegisterModel registerModel = new RegisterModel();
            registerModel.Name=Name;
            var result = await _userManager.FindByNameAsync(Name).ConfigureAwait(false);
            if (result == null)
                return null;
            else
            {
                return registerModel;
            }
        }

        public async Task<AuthorizationData> GetByAuthorizationDataUserIdAsync(string token)
        {
            return await _dbContext.AuthorizationData.Where(op => op.token == token).FirstOrDefaultAsync();
        }

        public async Task<GenerateCaptchaCode> GetByGenerateCaptchaCodeAsync(string captchaCode)
        {
            return await _dbContext.GenerateCaptchaCode.Where(op => op.CaptchaCode == captchaCode).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PasswordChangeHistory>> GetByPasswordChangeHistoryAsync(string UserPassword)
        {
            return await _dbContext.PasswordChangeHistory.Where(op => op.UserPassword == UserPassword).ToListAsync();
        }

        public async Task<List<UserPermission>> GetUserPermissions(string userId)
        {
            return await _dbContext.UserPermissions
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }


        public async Task<RegisterModel> RegisterAsync(RegisterModel registerModel)
        {
            var user = new ApplicationUser
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,
                Name = registerModel.Name
            };
            await _userManager.CreateAsync(user, registerModel.Password);
            return registerModel;
        }

        public async Task<UpdatePermissionsRequest> UpdateUserPermissions(UpdatePermissionsRequest request)
        {
            var current = await _dbContext.UserPermissions
                .Where(p => p.UserId == request.UserId)
                .ToListAsync();

            _dbContext.UserPermissions.RemoveRange(current);

            var newPermissions = request.Permissions.Select(p => new UserPermission
            {
                UserId = request.UserId,
                Permission = p
            });

            await _dbContext.UserPermissions.AddRangeAsync(newPermissions);
            await _dbContext.SaveChangesAsync();
            return request;
        }
    }
}
