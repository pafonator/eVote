using System.Net.Http;
using System.Text.Json;
using eVote.src.Controller;
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

        public List<TableRow> TableData { get; private set; } = new();

        public async Task OnGetAsync()
        {
            await FillTable();
        }

        public async Task FillTable()
        {
            var response = await _httpClient.GetAsync("api/evote/table");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                TableData = JsonSerializer.Deserialize<List<TableRow>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<TableRow>();
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to fetch table data: {message}");
                TableData = new List<TableRow>(); // Ensure we have an empty list on failure
            }
        }
    }
}
