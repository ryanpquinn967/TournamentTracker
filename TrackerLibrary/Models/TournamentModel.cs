using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public class TournamentModel
    {
        public event EventHandler<DateTime> OnTournamentComplete;  // create event
        /// <summary>
        /// the unique identifer
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// represents the name of the tournament
        /// </summary>
        public string TournamentName { get; set; }
        /// <summary>
        /// represents the entry fee for the tournament
        /// </summary>
        public decimal EntryFee { get; set; }

        /// <summary>
        /// represents a list of all the teams in the tournament
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();

        /// <summary>
        /// represents a list of the prizes for the tournament
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();

        /// <summary>
        /// each round has a list of the matchups, rounds is a list of lists of matchups
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();

        public void CompleteTournament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);

        }
    }
}
