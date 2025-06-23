using eVote.Pages;
using eVote.src.Model.DTO;

namespace eVote.src.ClientSide
{
    public class CurrentUserService
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly HttpClient _httpClient;
        private UserInfo? _currentUser;
        private bool _loaded = false;

        public CurrentUserService(ILogger<LoginModel> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("eVoteAPI"); // Ensure the client is created  
        }

        public async Task<UserInfo?> GetCachedUserAsync()
        {
            if (_loaded) return _currentUser;
            return await LoadCurrentUserAsync();
        }

        public async Task<UserInfo?> LoadCurrentUserAsync()
        {
            try
            {
                var user = await _httpClient.GetFromJsonAsync<UserInfo>("api/evote/user/info");
                _currentUser = user;
                _loaded = true;
            }
            catch
            {
                _currentUser = null;
            }
            return _currentUser;
        }

        public void ClearCache()
        {
            _loaded = false;
            _currentUser = null;
        }
    }
}
