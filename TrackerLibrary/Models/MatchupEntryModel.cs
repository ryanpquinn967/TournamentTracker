using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
        /// <summary>
        /// The unique identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Unique identifier for the team
        /// </summary>
        public int TeamCompetingId { get; set; }

        /// <summary>
        /// Represents one team competing
        /// </summary>
        public TeamModel TeamCompeting { get; set; }
        /// <summary>
        /// Represents the teams score
        /// </summary>
        public double Score { get; set; }
        
        /// <summary>
        /// the unique identifier for the parent matchup (team)
        /// </summary>
        public int ParentMatchupId { get; set; }
        /// <summary>
        /// The parent that includes both team in the matchup
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }

    }
}
