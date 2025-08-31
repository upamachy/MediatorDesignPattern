using DemoLibrary.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorUI.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LogIn loginModel);
        Task<AuthResult> RegisterAsync(Register registerModel);
        Task<AuthResult> LogoutAsync();
        Task<UserInfo?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(LogIn loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new AuthResult 
                    { 
                        Success = true, 
                        Message = result?.Message ?? "Login successful",
                        UserInfo = new UserInfo
                        {
                            UserId = result?.UserId ?? "",
                            UserName = result?.UserName ?? "",
                            Email = result?.Email ?? "",
                            FullName = result?.FullName ?? "",
                            Roles = result?.Roles ?? new List<string>()
                        }
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new AuthResult { Success = false, Message = errorContent };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new AuthResult { Success = false, Message = "An error occurred during login" };
            }
        }

        public async Task<AuthResult> RegisterAsync(Register registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<RegisterResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new AuthResult 
                    { 
                        Success = true, 
                        Message = result?.Message ?? "Registration successful"
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new AuthResult { Success = false, Message = errorContent };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return new AuthResult { Success = false, Message = "An error occurred during registration" };
            }
        }

        public async Task<AuthResult> LogoutAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/auth/logout", null);
                
                if (response.IsSuccessStatusCode)
                {
                    return new AuthResult { Success = true, Message = "Logout successful" };
                }
                else
                {
                    return new AuthResult { Success = false, Message = "Logout failed" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return new AuthResult { Success = false, Message = "An error occurred during logout" };
            }
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            try
            {
                // This would typically get user info from a secure endpoint
                // For now, return null as this requires session/token management
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                return user != null;
            }
            catch
            {
                return false;
            }
        }
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserInfo? UserInfo { get; set; }
    }

    public class UserInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class RegisterResponse
    {
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}