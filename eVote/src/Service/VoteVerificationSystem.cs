using eVote.src.Controller;
using eVote.src.Models;

namespace eVote.src.Service
{
    public class VoteVerificationSystem
    {
        public Dictionary<UserId, int> CountAllCandidateVotes()
        {

            Dictionary<UserId, int> candidateVotes = new Dictionary<UserId, int>();

            var votes = EVoteController.GetAllVotes();
            foreach (var vote in votes)
            {
                // The voted is a candidate
                User candidate = EVoteController.GetUser(vote.CandidateId);
                if (!candidate.IsCandidate)
                    continue;

                // The voter is not a candidate
                User voter = EVoteController.GetUser(vote.VoterId);
                if (voter.IsCandidate)
                    continue;

                int voteCount = EVoteController.GetVotesOfUser(vote.VoterId).Count;
                if (voteCount > 2)
                    throw new InvalidOperationException("A user can only vote once.");

                //Add Counter
                if (candidateVotes.ContainsKey(vote.CandidateId))
                {
                    candidateVotes[vote.CandidateId]++;
                }
                else
                {
                    candidateVotes[vote.CandidateId] = 1;
                }
            }
            // Count the number of votes for a specific candidate
            return candidateVotes;
        }


        public int CountCandidateVotes(UserId candidateId) {

            User candidate = EVoteController.GetUser(candidateId);
            if (!candidate.IsCandidate)
                throw new ArgumentException("The user is not a candidate."); 
            
            Dictionary<UserId, int> candidateVotes = CountAllCandidateVotes();
            if (!candidateVotes.ContainsKey(candidateId))
                return 0;
            return candidateVotes[candidateId];
        }
    }
}
