using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    {
        /// <summary>
        /// The unique identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// represents both teams in a matchup
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        // represents the team of the winner of the round

        /// <summary>
        /// Winner Id used whwn loading from sql
        /// </summary>
        public int WinnerId { get; set; }

        public TeamModel Winner { get; set; }
        /// <summary>
        /// represents the current round number
        /// </summary>
        public int MatchupRound { get; set; }

        public string DisplayName 
        {
            get
            {
                string output = "";
                foreach(MatchupEntryModel mem in Entries)
                {
                    if (mem.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = mem.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output = output + $" vs. { mem.TeamCompeting.TeamName}";
                        } 
                    } 
                    else
                    {
                        output = "Matchup Unknown";
                        break;
                    }
                }
                return output;
            }
        }

    }
}
