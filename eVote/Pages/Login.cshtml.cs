using System.Net.Http;
using System.Security.Claims;
using eVote.src.Controller;
using eVote.src.Model.DTO;
using eVote.src.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace eVote.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly HttpClient _httpClient;
        public LoginModel(ILogger<LoginModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("eVoteAPI"); // Ensure the client is created
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet() {

        }

        public async Task<IActionResult> OnPostLoginAsync(string action)
        {
            var input = JsonContent.Create(new UserCredentials { Email = Email, Password = Password });
            var response = await _httpClient.PostAsync("api/evote/user/login", input);
            if (response.IsSuccessStatusCode)
            {
                var tokenData = await response.Content.ReadFromJsonAsync<TokenAuthentication>();

                // Store the token in a secure, HttpOnly cookie
                Response.Cookies.Append("AuthToken", tokenData.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // only over HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return RedirectToPage("/Index");
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                ErrorMessage = message;
                _logger.LogError($"Login failed: {message}");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostRegisterAsync(string action)
        {
            var input = JsonContent.Create(new UserCredentials{ Email = Email, Password = Password});
            var response = await _httpClient.PostAsync("api/evote/user/register", input);
            if (response.IsSuccessStatusCode)
            {
                var tokenData = await response.Content.ReadFromJsonAsync<TokenAuthentication>();

                // Store the token in a secure, HttpOnly cookie
                Response.Cookies.Append("AuthToken", tokenData.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // only over HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return RedirectToPage("/Index");
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                ErrorMessage = message;
                _logger.LogError($"Registration failed: {message}");
            }
            return Page();
        }
    }
}
