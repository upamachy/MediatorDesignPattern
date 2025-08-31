using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using BlazorUI.Services;

namespace BlazorUI.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync();
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                    };

                    // Add role claims
                    claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "custom"));
                }
                else
                {
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task LoginAsync(UserInfo userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                new Claim(ClaimTypes.Name, userInfo.UserName),
                new Claim(ClaimTypes.Email, userInfo.Email),
            };

            claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "custom"));
            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public async Task LogoutAsync()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}