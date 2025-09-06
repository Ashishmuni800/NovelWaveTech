using Application.DTO;
using Application.Permissions;
using Application.ServiceInterface;
using Application.ViewModel;
using Azure.Core;
using Domain.Model;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace NovelWaveTechAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _EmailServices;
        private readonly IServiceInfra _userAuthService;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService EmailServices, IServiceInfra userAuthService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _EmailServices = EmailServices;
            _userAuthService = userAuthService;
        }
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                if (model.Count() == 50) break;
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(model);
        }
        [HttpGet]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                AllRoles = allRoles,
                AssignedRoles = userRoles.ToList()
            };

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoles([FromBody] UpdateRolesRequestDTO request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(request.Roles).ToList();
            var rolesToAdd = request.Roles.Except(currentRoles).ToList();

            // Prevent self-admin removal
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user.Id == currentUserId && currentRoles.Contains("Admin") && !request.Roles.Contains("Admin"))
            {
                return BadRequest("You cannot remove your own Admin role.");
            }

            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                    return BadRequest("Failed to remove old roles.");
            }

            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                    return BadRequest("Failed to assign new roles.");
            }

            //await _EmailServices.SendEmailAsync(user.Email, "Roles Changed",
            //$"Your roles were updated.<br/>Added: {string.Join(", ", rolesToAdd)}<br/>Removed: {string.Join(", ", rolesToRemove)}");

            // Audit log (replace with DB if needed)
            //_logger.LogInformation($"User {user.Email} roles updated by {User.Identity.Name}. " +
            //    $"Added: {string.Join(", ", rolesToAdd)}, Removed: {string.Join(", ", rolesToRemove)}");

            return Ok("Roles updated.");
        }


        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserPermissions(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("Invalid or null user id");

            var assigned = await _userAuthService.AuthService.GetUserPermissions(userId);
            var user = await _userManager.FindByIdAsync(userId);

            var listdata = new List<string>();

            foreach (var item in assigned)
            {
                listdata.Add(item.Permission);
            }

            var getdata = new PermissionMatrixViewModel()
            {
                AllPermissions = Permissions.All,
                UserId = (assigned != null && assigned.Any() && !string.IsNullOrEmpty(assigned[0].UserId))
                ? assigned[0].UserId
                : userId,
                Email = user.Email, //assigned?.Select(a => a.Permission).ToList() ?? new List<string>()
                AssignedPermissions = listdata
            };
            return Ok(getdata);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPermissions([FromBody] UpdatePermissionsRequestDTO request)
        {
            if (request.UserId == null) return BadRequest("Invalid input");
            //if (request.Permissions.Count<=0) return BadRequest("Invalid input");
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return NotFound();
            var UpdatePermission = await _userAuthService.AuthService.UpdateUserPermissions(request);
            if (UpdatePermission == null) return BadRequest("Failed to update Permissios.");
            return Ok("Permissions updated");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserLoginPermissions()
        {
            var roles = User.Claims
                       .Where(c => c.Type == "permission")
                       .Select(c => c.Value)
                       .ToList();
            return Ok(new
            {
                Roles = roles
            });
        }
    }
}
