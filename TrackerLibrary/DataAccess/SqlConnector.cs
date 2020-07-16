using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using TrackerLibrary.Models;
using Dapper;
using System.Linq;
using System.Runtime.CompilerServices;

//@PlaceNumber int,
//@PlaceName nvarchar(100),
//@PrizeAmount money,
//@PrizePercentage float,
//@id int = 0 output

namespace TrackerLibrary.DataAccess
{
    class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";
        /// <summary>
        /// Saves a prize Model to the database
        /// </summary>
        /// <param name="model">the prize information</param>
        /// <returns></returns>
        public void CreatePrize(PrizeModel model)
        {
 
            // use this connection for a short time between the parenthesis (the get rid of via GC).
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");

            }
        }

        /// <summary>
        /// Saves a person Model to the database
        /// </summary>
        /// <param name="model">the person information</param>
        /// <returns></returns>
        public void CreatePerson(PersonModel model)
        {
            // use this connection for a short time between the parenthesis (the get rid of via GC).
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellphoneNumber);
                p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");

            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }
            return output;
        }

        public void CreateTeam(TeamModel model)
        {
            // use this connection for a short time between the parenthesis (the get rid of via GC).
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@Id");

                foreach (PersonModel tm in model.TeamMembers )
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", tm.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

            }
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("spTeams_GetAll").ToList();

                // loop through and get team members for every team.
                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);
                    // we have to pass a parameter to the query by using comma paramater ,p
                    team.TeamMembers = connection.Query<PersonModel>("spTeamMembers_GetByTeam",p, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return output;
            
        }

        public void CreateTournament(TournamentModel model)
        {
             //use this connection for a short time between the parenthesis (the get rid of via GC).
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(model,connection);

                SaveTournamentPrizes(model, connection);

                SaveTournamentEntries(model, connection);

                SaveTournamentRounds(model, connection);

                TournamentLogic.UpdateTournamentResults(model);
                //return model;
            }

        }
        private void SaveTournamentRounds(TournamentModel model, IDbConnection connection)
        {

            // int Id,
            // int - Id^TeamModel - TeamCompeting^double - score^MatchupModel - Parent|
            // int - Id^TeamModel - TeamCompeting^double - score^MatchupModel - Parent,
            // TeamModel - Winner, 
            // int - Round

            // 
            // List<List<MatchupModels>> Rounds
            // List<MatchupEntryModel> Entries

            // Loop through the rounds
            foreach (List<MatchupModel> round in model.Rounds)
            {
                // Loop through the matchups
                foreach (MatchupModel matchup in round)
                {
                    // Save the matchup
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", model.Id);
                    p.Add("@MatchupRound", matchup.MatchupRound);
                    p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_insert", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("@Id");

                    foreach(MatchupEntryModel entry in matchup.Entries)
                    {
                        // Save the matchup entries
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);
                        p.Add("@ParentMatchupId", entry.ParentMatchupId);
                        if (entry.TeamCompeting == null)
                        {
                            p.Add("@TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);

                        //matchup.Id = p.Get<int>("@Id");

                    }

                }
            }

        }
        private void SaveTournament(TournamentModel model, IDbConnection connection)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", model.TournamentName);
            p.Add("@EntryFee", model.EntryFee);
            p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@Id");
        }
        private void SaveTournamentPrizes(TournamentModel model, IDbConnection connection)
        {
            // create the insert in sql while coding this.
            foreach (PrizeModel pm in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@PrizeId", pm.Id);
                p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentEntries(TournamentModel model, IDbConnection connection)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", model.Id);
                p.Add("@TeamId", tm.Id);
                p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }

        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

                foreach( TournamentModel t in output)
                {
                    // populate prizes
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", t.Id);
                    // we have to pass a parameter to the query by using comma paramater ,p
                    t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament",p, commandType: CommandType.StoredProcedure).ToList();

                    // populate Teams
                    t.EnteredTeams = connection.Query<TeamModel>("dbo.spTeams_GetByTournament",p, commandType: CommandType.StoredProcedure).ToList();

                    // loop through and get team members for every team.
                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        p = new DynamicParameters();
                        p.Add("@TeamId", team.Id);
                        // we have to pass a parameter to the query by using comma paramater ,p
                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }

                    p = new DynamicParameters();
                    p.Add("@TournamentId", t.Id);

                    // spMatchups_GetByTournament
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    // Populate Rounds
                    foreach (MatchupModel m in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", m.Id);

                        m.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        // Loop through each entry and populate models for team Competing
                        List<TeamModel> allTeams = GetTeam_All();

                        // populate the team model based on the winner id.
                        if (m.WinnerId > 0)
                        {
                            m.Winner = allTeams.Where(x => x.Id == m.WinnerId).First();
                        }
                        foreach(var me in m.Entries)
                        {
                            if (me.TeamCompetingId > 0) // id of 0 is not a valid id
                            {
                                // find team and populate it.
                                me.TeamCompeting = allTeams.Where(x => x.Id == me.TeamCompetingId).First();

                            }
                            // Loop through each entry and populate models for MatchupModel
                            if (me.ParentMatchupId > 0)
                            {
                                me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
                            }
                        }

                    }
                    // Populate the rounds, List<List<MatchupModel>>
                    List<MatchupModel> currRow = new List<MatchupModel>();
                    int currRound = 1;
                    foreach (MatchupModel m in matchups)
                    {
                        if (m.MatchupRound > currRound) // round changed, save round list
                        {
                            t.Rounds.Add(currRow);
                            currRow = new List<MatchupModel>();
                            currRound += 1;
                        }
                        currRow.Add(m);  // same round so add matchup to curr round

                    }
                    t.Rounds.Add(currRow); // Add the last row.


                }
            }
            return output;
        }

        public void UpdateMatchup(MatchupModel model)
        {

            // dbo.spMatchups_Update
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                if (model.Winner != null)
                {
                    p.Add("@Id", model.Id);
                    p.Add("@WinnerId", model.Winner.Id);
                    connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure);
                }

                // dbo.spMatchupEntries_Update Id, TeamCompeting, Score
                foreach (MatchupEntryModel me in model.Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        p = new DynamicParameters();
                        p.Add("@Id", me.Id);
                        p.Add("@TeamCompetingId", me.TeamCompeting.Id);
                        p.Add("@Score", me.Score);
                        connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure); 
                    }
                }

            }


        }

        public void CompleteTournament(TournamentModel model)
        {
            // flag active bit to false.
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@Id", model.Id);
                connection.Execute("dbo.spTournaments_Complete", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
