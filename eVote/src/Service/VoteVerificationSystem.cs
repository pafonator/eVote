using eVote.src.Controller;
using eVote.src.Models;
using eVote.src.Repository;

namespace eVote.src.Service
{
    public class VoteVerificationSystem
    {
        public Dictionary<UserId, int> CountAllCandidateVotes()
        {

            Dictionary<UserId, int> candidateVotes = new Dictionary<UserId, int>();

            var votes = DbAccess.GetAllVotesAsync();
            foreach (var vote in votes.Result)
            {
                // The voted is a candidate
                User? candidate = DbAccess.GetUserAsync(vote.CandidateId).Result;
                if (candidate == null || !candidate.IsCandidate)
                    continue;

                // The voter is not a candidate
                User? voter = DbAccess.GetUserAsync(vote.VoterId).Result;
                if (voter == null || voter.IsCandidate)
                    continue;

                int voteCount = DbAccess.GetVotesOfUserAsync(vote.VoterId).Result.Count;
                if (voteCount > 2)
                    continue;
                    //throw new InvalidOperationException("A user can only vote once. All votes are invalidated");

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

            User? candidate = DbAccess.GetUserAsync(candidateId).Result;
            if (candidate == null || !candidate.IsCandidate)
                throw new ArgumentException("The user is not a candidate."); 
            
            Dictionary<UserId, int> candidateVotes = CountAllCandidateVotes();
            if (!candidateVotes.ContainsKey(candidateId))
                return 0;
            return candidateVotes[candidateId];
        }
    }
}
