using System.Net.Http;
using System.Text.Json;
using eVote.src.ClientSide;
using eVote.src.Controller;
using eVote.src.Model;
using eVote.src.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVote.Pages
{
    public class Index : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly CurrentUserService _currentUser;
        public Index(ILogger<LoginModel> logger,
            IHttpClientFactory httpClientFactory,
            CurrentUserService currentUser)
        {
            _currentUser = currentUser;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("eVoteAPI"); // Ensure the client is created
        }

        public UserInfo? CurrentUser { get; private set; } = new();
        public List<UserId> UserCandidateVotes { get; private set; } = new();
        public List<UserVoteInfo> TableData { get; private set; } = new();
        public int RemainingVotes { get; private set; } = 0;
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await Update();
        }

        public async Task Update()
        {
            CurrentUser = await _currentUser.GetCachedUserAsync();
            UserCandidateVotes.Clear(); // Clear previous votes
            if (CurrentUser != null)
            {
                var votes = await _httpClient.GetFromJsonAsync<List<Vote>>("api/evote/user/votes");
                foreach (var vote in votes)
                {
                    UserCandidateVotes.Add(vote.CandidateId);
                }
            }
            await FillTable();
            await GetNbRemainingVotes();
        }

        public async Task FillTable()
        {
            var response = await _httpClient.GetAsync("api/evote/table");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                TableData = JsonSerializer.Deserialize<List<UserVoteInfo>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<UserVoteInfo>();
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to fetch table data: {message}");
                TableData = new List<UserVoteInfo>(); // Ensure we have an empty list on failure
            }
        }

        public async Task GetNbRemainingVotes()
        {
            var response = await _httpClient.GetAsync("api/evote/remainingVotes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                RemainingVotes = JsonSerializer.Deserialize<int>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                _logger.LogError($"Failed to get remaining votes data");
                RemainingVotes = 0; // Default to 0 on failure
            }
        }

        
        public async Task<IActionResult> OnPostVoteForAsync(UserId id)
        {
            var input = JsonContent.Create(id);
            var response = await _httpClient.PostAsync("api/evote/user/addVote", input);
            if (!response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Vote failed: {message}");
                ErrorMessage = message; // Set error message to display in the UI
            }
            await Update();
            return Page();
        }
        
        public async Task<IActionResult> OnPostRemoveVoteAsync(Guid id)
        {
            var input = JsonContent.Create(id);
            var response = await _httpClient.PostAsync("api/evote/user/removeVote", input);
            if (!response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Remove vote failed: {message}");
                ErrorMessage = message; // Set error message to display in the UI
            }
            await Update();
            return Page();
        }
    }
}
