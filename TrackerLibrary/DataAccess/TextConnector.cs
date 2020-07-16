using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess;
using TrackerLibrary.DataAccess.TextHelpers;
using System.Linq;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        /// <summary>
        /// get the prize model from the data base.
        /// </summary>
        /// <param name="model">the prize</param>
        /// <returns></returns>
        public void CreatePrize(PrizeModel model)
        {
            // * Load the text file
            // * Convert the text to List<PrizeModel>
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Find the max ID
            int currentId = 1;
            if (prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            currentId += 1;

            // * Add the new record with the new Id(max + 1)
            prizes.Add(model);

            // convert the prizes to List<string>
            //Save the List<string> to the text file
            prizes.SaveToPrizeFile();

        }
        /// <summary>
        /// get the person model from the data base.
        /// </summary>
        /// <param name="model">the person</param>
        /// <returns></returns>
        public void CreatePerson(PersonModel model)
        {
            // * Load the text file
            // * Convert the text to List<PrizeModel>
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // Find the max ID
            int currentId = 1;
            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            currentId += 1;

            // * Add the new record with the new Id(max + 1)
            people.Add(model);

            // convert the people to List<string>
            //Save the List<string> to the text file
            people.SaveToPeopleFile();

        }

        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public void CreateTeam(TeamModel model)
        {
            // * Load the text file
            // * Convert the text to List<TeamModel>
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();

            // Find the max ID
            int currentId = 1;
            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            currentId += 1;  // TODO - CHECK THIS

            // * Add the new record with the new Id(max + 1)
            teams.Add(model);

            // convert the teams to List<string>
            //Save the List<string> to the text file
            teams.SaveToTeamFile();

        }

        public List<TeamModel> GetTeam_All()
        {
            // * Load the text file
            // * Convert the text to List<TeamModel>
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
            return teams;
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();

            // Find the max ID
            int currentId = 1;
            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            //currentId += 1;  // TODO - CHECK THIS

            model.SaveRoundsToFile();

            // * Add the new record with the new Id(max + 1)
            tournaments.Add(model);

            // convert thetournaments to List<string>
            //Save the List<string> to the text file
            tournaments.SaveToTournamentFile();

            TournamentLogic.UpdateTournamentResults(model);

        }

        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }

        public void CompleteTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();


            // * remove our tournament
            tournaments.Remove(model);

            // convert thetournaments to List<string>
            //Save the List<string> to the text file
            tournaments.SaveToTournamentFile();

            TournamentLogic.UpdateTournamentResults(model);
        }
    }
}
