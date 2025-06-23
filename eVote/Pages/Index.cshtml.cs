using System.Net.Http;
using System.Text.Json;
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
        public Index(ILogger<LoginModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("eVoteAPI"); // Ensure the client is created
        }

        public UserInfo CurrentUser { get; private set; } = new();
        public List<UserId> UserCandidateVotes { get; private set; } = new();
        public List<UserVoteInfo> TableData { get; private set; } = new();
        public int RemainingVotes { get; private set; } = 0;

        public async Task OnGetAsync()
        {
            await Update();
        }

        public async Task Update()
        {
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


    }
}
