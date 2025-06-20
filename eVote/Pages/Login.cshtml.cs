using System.Net.Http;
using eVote.src.Controller;
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
                Console.WriteLine($"Login: {Email} {Password}");
                // Handle login
                //var user = DbAccess.GetUserAsync(Email).Result;
                //if (user == null || user.Password != Password)
                //{
                //    ErrorMessage = "Invalid email or password.";
                //    return Page();
                //}

                // You could set session/cookie info here

                //return RedirectToPage("/Index"); // Or your protected page
            }
            else if (action == "register")
            {
                Console.WriteLine($"Register: {Email} {Password}");
                // Redirect to registration page
                //return RedirectToPage("/Register");
            }
            else if (action == "test")
            {
                var response = await _httpClient.GetAsync("api/evote/test");
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response: {message}");
                }
                else
                {
                    ErrorMessage = "API call failed.";
                }
            }

            return Page();
        }
    }
}
