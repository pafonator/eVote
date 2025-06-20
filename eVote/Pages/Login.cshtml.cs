using System.Net.Http;
using eVote.src.Controller;
using eVote.src.Model.DTO;
using eVote.src.Repository;
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

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (action == "login")
            {
                var input = JsonContent.Create(new UserCredentials { Email = Email, Password = Password });
                var response = await _httpClient.PostAsync("api/evote/user/login", input);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    ErrorMessage = message;
                    _logger.LogError($"Login failed: {message}");
                }
            }
            else if (action == "register")
            {
                var input = JsonContent.Create(new UserCredentials{ Email = Email, Password = Password});
                var response = await _httpClient.PostAsync("api/evote/user/register", input);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Register: {Email} {Password}");
                    return RedirectToPage("/Index");
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    ErrorMessage = message;
                    _logger.LogError($"Registration failed: {message}");
                }
            }

            return Page();
        }
    }
}
