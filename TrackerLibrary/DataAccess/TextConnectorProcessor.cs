using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName) // PrizeModels.csv
        {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        }

        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList(); // takes a full path and returns list of strings

        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(','); // the lines are split by commas

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);

                output.Add(p);
            }
            return output;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
        {
            // Id, TeamName, list of Ids sperated by the pipe
            // 3, Tim's Team,1|3|5

            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(','); // the lines are split by commas
                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                // get the person Ids in the team
                string[] personIds = cols[2].Split('|');

                foreach(string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);

            }
            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines)
        {
            // Id, Tournament Name, EntryFee,id(team)|id(team)|id(team),Id(prize)|Id(prize)|Id(prize),id^id^id|id^id^id|id^id^id
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig
                .TeamsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTeamModels();
            List<PrizeModel> prizes = GlobalConfig
                .PrizesFile
                .FullFilePath()
                .LoadFile()
                .ConvertToPrizeModels();
            // load the matchups
            List<MatchupModel> matchups = GlobalConfig
                .MatchupFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(','); //The lines are split by commas
                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                // get the Team Ids in the Tournament
                string[] teamIds = cols[3].Split('|');
                foreach (string id in teamIds)
                {
                    // find me the team with id that matches this id.  give me first one
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                if (cols[4].Length > 0)
                {
                    // get the prizes in the tournament
                    string[] prizeIds = cols[4].Split('|');
                    foreach (string id in prizeIds)
                    {
                        // find me the team with id that matches this id.  give me first one
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                    } 
                }

                // CAPTURE ROUNDS INFORMATION
                string[] rounds = cols[5].Split('|');
                List<MatchupModel> ms;

                foreach (string round in rounds)
                {
                    ms = new List<MatchupModel>();
                    string[] msText = round.Split('^');
                    foreach(string mmId in msText)
                    {
                        // find the matchup model that matches the id
                        ms.Add(matchups.Where(x => x.Id == int.Parse(mmId)).First());
                    }
                    // and a list of matchup models for each round to the list of list of matchup models
                    tm.Rounds.Add(ms);
                    
                }
                output.Add(tm);


            }
            return output;
        }
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(','); // the lines are split by commas

                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];

                output.Add(p);
            }
            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }

            File.WriteAllLines(GlobalConfig.PrizesFile.FullFilePath(),lines);

        }

        public static void SaveToTeamFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {
                lines.Add($"{ t.Id },{ t.TeamName},{ ConvertPeopleListToString(t.TeamMembers) }");
            }
            File.WriteAllLines(GlobalConfig.TeamsFile.FullFilePath(), lines);

        }
        public static void SaveRoundsToFile(this TournamentModel model)
        {
            // Loop through each round
            // Loop through each matchup
            // Get the id for the new matchup and save the record
            // loop through each entry, get the id, and save it

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    // load all of the matchups from file
                    // get the top id and add one
                    // store the id
                    // save the matchup record
                    matchup.SaveMatchupToFile();

                    

                }
            }
        }
        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach (string id in ids)
            {
                foreach(string entry in entries)
                {
                    string[] cols = entry.Split(',');
                    // if match, this is row i need
                    if (cols[0] == id) 
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }

            output = matchingEntries.ConvertToMatchupEntryModels();
            return output;
            
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            // Id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(','); // the lines are split by commas

                MatchupEntryModel mem = new MatchupEntryModel();
                mem.Id = int.Parse(cols[0]);
                if (cols[1].Length == 0)
                {
                    mem.TeamCompeting = null;
                } 
                else
                {
                    mem.TeamCompeting = lookupTeamById(int.Parse(cols[1]));
                }
                mem.Score = double.Parse(cols[2]);
                int parentId = 0;
                if (int.TryParse(cols[3],out parentId)) {
                    mem.ParentMatchup = lookupMatchupById(parentId);
                } 
                else
                {
                    mem.ParentMatchup = null;
                }
                

                output.Add(mem);
            }
            return output;
        }
        private static MatchupModel lookupMatchupById(int id)
        {
            //List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();
            List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();

            foreach(string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }
            //return matchups.Where(x => x.Id == id).First();
            return null;
        }

        private static TeamModel lookupTeamById(int id)
        {
            //List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(GlobalConfig.PeopleFile);
            List<string> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile();

            foreach(string team in teams)
            {
                string[] cols = team.Split(',');
                if(cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels().First();
                }
            }
            return null;
            //return teams.Where(x => x.Id == id).First();
        }
        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            //Id=0,Entries=1(pipedelimited),Winner=2, MatchupRound=3
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(','); // the lines are split by commas

                MatchupModel m = new MatchupModel();
                m.Id = int.Parse(cols[0]);
                // load entries from somewhere.
                m.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                if (cols[2].Length == 0)
                {
                    m.Winner = null;
                }
                else
                {
                    m.Winner = lookupTeamById(int.Parse(cols[2]));
                }
                m.MatchupRound = int.Parse(cols[3]);

                output.Add(m);
            }
            return output;
        }
        public static void SaveMatchupToFile(this MatchupModel matchup)
        {
            // load the matchups
            List<MatchupModel> matchups = GlobalConfig
                .MatchupFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupModels();

            // check for maximum Id
            int currentId = 1;
            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }
            matchup.Id = currentId;
            matchups.Add(matchup);

            // save matchupEntry's to the textfile
            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                // save matchup entries to the textfile
                entry.SaveEntryToFile();
            }
            
            // save to file
            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                //Id=0,Entries=1(pipedelimited),Winner=2, MatchupRound=3
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
            }
            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            // load the matchups
            List<MatchupModel> matchups = GlobalConfig
                .MatchupFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel();
            // take old matchup out
            foreach(MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }
            }
            matchups.Remove(oldMatchup);
            matchups.Add(matchup);

            // save matchupEntry's to the textfile
            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                // save matchup entries to the textfile
                entry.UpdateEntryToFile();
            }

            // save to file
            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                //Id=0,Entries=1(pipedelimited),Winner=2, MatchupRound=3
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
            }
            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }
        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            if (entries.Count == 0)
            {
                return "";
            }
            foreach (MatchupEntryModel mem in entries)
            {
                output += $"{ mem.Id }|";
            }
            // remove trailing '|'
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig
                .MatchupEntryFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupEntryModels();

            MatchupEntryModel oldEntry = new MatchupEntryModel();
            
            foreach(MatchupEntryModel e in entries)
            {
                if (e.Id == entry.Id)
                {
                    oldEntry = e;
                }
            }
            entries.Remove(oldEntry);
            entries.Add(entry);

            // save to the file here
            List<string> lines = new List<string>();

            foreach (MatchupEntryModel mem in entries)
            {
                string parent = "";
                if (mem.ParentMatchup != null)
                {
                    parent = mem.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (mem.TeamCompeting != null)
                {
                    teamCompeting = mem.TeamCompeting.Id.ToString();
                }
                // Id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3
                lines.Add($"{ mem.Id },{teamCompeting},{ mem.Score },{ parent }");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);

        }
        public static void SaveEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig
                .MatchupEntryFile
                .FullFilePath()
                .LoadFile()
                .ConvertToMatchupEntryModels();

            // find max id
            int currentId = 1; 
            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }

            entry.Id = currentId;
            entries.Add(entry);
            
            // save to the file here
            List<string> lines = new List<string>();

            foreach (MatchupEntryModel mem in entries)
            {
                string parent = "";
                if (mem.ParentMatchup != null)
                {
                    parent = mem.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (mem.TeamCompeting != null)
                {
                    teamCompeting = mem.TeamCompeting.Id.ToString();
                }
                // Id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3
                lines.Add($"{ mem.Id },{teamCompeting},{ mem.Score },{ parent }");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);

        }
        public static void SaveToTournamentFile(this List<TournamentModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tm in models)
            {
                // @ symbol turns string into a literal so we can now break lines with crlfs
                lines.Add($"{ tm.Id },{ tm.TournamentName},{ tm.EntryFee },{ ConvertTeamListToString(tm.EnteredTeams) },{ ConvertPrizeListToString(tm.Prizes) },{ ConvertRoundsListToString(tm.Rounds)}");
            }
            File.WriteAllLines(GlobalConfig.TournamentFile.FullFilePath(), lines);
        }

        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            if (rounds.Count == 0)
            {
                return "";
            }
            foreach (List<MatchupModel> lm in rounds)
            {
                output += $"{ ConvertMatchupListToString(lm) }|";
            }
            // remove trailing '|'
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupListToString(List<MatchupModel> matchUps)
        {
            string output = "";

            if (matchUps.Count == 0)
            {
                return "";
            }
            foreach (MatchupModel mm in matchUps)
            {
                output += $"{ mm.Id }^";
            }
            // remove trailing '|'
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }
            foreach (PrizeModel p in prizes)
            {
                output += $"{ p.Id }|";
            }
            // remove trailing '|'
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }
            foreach (TeamModel t in teams)
            {
                output += $"{ t.Id }|";
            }
            // remove trailing '|'
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        private static string ConvertPeopleListToString(List<PersonModel> people) 
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }
            foreach (PersonModel p in people)
            {
                output += $"{ p.Id }|";
            }
            // remove trailing '|'
            output = output.Substring(0, output.Length - 1);

            return output;
        }


        public static void SaveToPeopleFile(this List<PersonModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
            {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellphoneNumber }");
            }

            File.WriteAllLines(GlobalConfig.PeopleFile.FullFilePath(), lines);

        }
    }
}
