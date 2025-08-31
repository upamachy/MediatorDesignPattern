using DemoLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
                return BadRequest(new { Message = "Username already exists" });

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
                return BadRequest(new { Message = "Email already exists" });

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} registered successfully", model.UserName);
                
                // Assign default "User" role if it exists
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }

                return Ok(new 
                { 
                    Message = "User registered successfully", 
                    UserId = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogIn model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("User {UserName} logged in successfully", model.UserName);

                return Ok(new 
                { 
                    Message = "Login successful", 
                    UserId = user.Id.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles
                });
            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new { Message = "User account is locked out" });
            }

            if (result.IsNotAllowed)
            {
                return Unauthorized(new { Message = "Login not allowed" });
            }

            return Unauthorized(new { Message = "Invalid username or password" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully");
            return Ok(new { Message = "Logout successful" });
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
                return BadRequest("Role name is required");

            var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
            if (roleExists)
                return BadRequest($"Role '{request.RoleName}' already exists");

            var role = new ApplicationRole
            {
                Name = request.RoleName,
                Description = request.Description ?? $"{request.RoleName} role",
                IsActive = true
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} created successfully", request.RoleName);
                return Ok(new { Message = $"Role '{request.RoleName}' created successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return NotFound("User not found");

            var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
            if (!roleExists)
                return NotFound("Role not found");

            var isInRole = await _userManager.IsInRoleAsync(user, request.RoleName);
            if (isInRole)
                return BadRequest($"User is already in role '{request.RoleName}'");

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} assigned to user {UserName}", request.RoleName, request.UserName);
                return Ok(new { Message = $"Role '{request.RoleName}' assigned to user '{request.UserName}' successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                Roles = roles
            });
        }

        [HttpGet("init-roles")]
        public async Task<IActionResult> InitializeRoles()
        {
            var defaultRoles = new[] { "Admin", "User", "Moderator" };
            var createdRoles = new List<string>();

            foreach (var roleName in defaultRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} role",
                        IsActive = true
                    };

                    var result = await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        createdRoles.Add(roleName);
                    }
                }
            }

            return Ok(new { Message = "Roles initialized", CreatedRoles = createdRoles });
        }
    }

    public class CreateRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class AssignRoleRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}