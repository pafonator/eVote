using Azure;
using eVote.src.Client;
using eVote.src.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVote.Pages
{
    public class AccountModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly CurrentUserService _currentUser;
        public AccountModel(ILogger<LoginModel> logger, 
            IHttpClientFactory httpClientFactory,
            CurrentUserService currentUser)
        {
            _currentUser = currentUser;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("eVoteAPI"); // Ensure the client is created  
        }

        public string CurrentUserEmail { get; set; }

        [BindProperty]
        public bool IsCandidate { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await UpdateUser();
        }

        public async Task UpdateUser()
        {
            var currentUser = await _currentUser.LoadCurrentUserAsync();
            if (currentUser != null)
            {
                CurrentUserEmail = currentUser.Email;
                IsCandidate = currentUser.IsCandidate;
            }
            else
            {
                ErrorMessage = "Unable to load user data";
                _logger.LogError(ErrorMessage);
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
                await UpdateUser(); // Refresh user info
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
            _currentUser.ClearCache();
            return RedirectToPage("/Login");
        }
    }
}
