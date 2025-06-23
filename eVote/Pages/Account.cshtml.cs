using Azure;
using eVote.src.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVote.Pages
{
    public class AccountModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly HttpClient _httpClient;
        public AccountModel(ILogger<LoginModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("eVoteAPI"); // Ensure the client is created  
            LoadCurrentUser().Wait();
        }

        public string CurrentUserEmail { get; set; }

        [BindProperty]
        public bool IsCandidate { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadCurrentUser();
        }

        public async Task LoadCurrentUser()
        {
            var response = await _httpClient.GetAsync("api/evote/user/info");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var currentUser = System.Text.Json.JsonSerializer.Deserialize<UserInfo>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (currentUser != null)
                {
                    CurrentUserEmail = currentUser.Email;
                    IsCandidate = currentUser.IsCandidate;
                }
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(message))
                {
                    ErrorMessage = "Unable to access user data";
                }
                else
                {
                    ErrorMessage = message;
                }
                _logger.LogError($"Failed to load user data: {ErrorMessage}");
            }
        }


        public async Task<IActionResult> OnPostCandidateToggleAsync()
        {
            HttpResponseMessage response;
            if (IsCandidate) {
                response = await _httpClient.PostAsJsonAsync("api/evote/user/unbecomeCandidate", new { });
            }
            else
            {
                response = await _httpClient.PostAsJsonAsync("api/evote/user/becomeCandidate", new { });
            }

            // Update UI
            if (response.IsSuccessStatusCode)
            {
                // Successfully toggled candidate status
                await LoadCurrentUser(); // Refresh user info
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                ErrorMessage = message;
                _logger.LogError($"Failed to change candidate Status: {message}");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync(string action)
        {
            // Delete the auth Cookie
            Response.Cookies.Delete("AuthToken");
            return RedirectToPage("/Login");
        }
    }
}
