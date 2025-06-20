using eVote.src.Controller;
using eVote.src.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eVote.Pages
{
    public class VoteTableModel : PageModel
    {
        public List<UserWithVotesDto> Users { get; private set; } = new();
        private readonly ILogger<VoteTableModel> _logger;
        public VoteTableModel(ILogger<VoteTableModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //Users = EVoteController.GetUsersWithVotesAsync().Result;
        }
    }
}
