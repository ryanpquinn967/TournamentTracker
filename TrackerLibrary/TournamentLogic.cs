using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        // 1 TODO - Wire up our matchups
        // 2 order our list randomly, who gets picked when is random, not order you put in.
        // 3 take the list and check if its big enough, otherwise add in byes (skips) (auto wins)
        // 3 2^n power, need 2, 2x2, 2x2x2, 2x2x2x2. etc.
        // 4 create first round of matchups
        // 5 create every round after that
        public static void CreateRounds(TournamentModel model)
        {
            // Order our list randomly
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            // Number of Rounds
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            // Number of Teams
            int byes = NumberOfByes(rounds,randomizedTeams.Count);

            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));
            CreateOtherRounds(model, rounds);

        }

        public static void UpdateTournamentResults(TournamentModel model)
        {
            int startingRound = model.CheckCurrentRound();
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    // if any entries have score not = zero, its a completed match OR a bye week
                    if ((rm.Winner == null) && rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1)
                    {
                        // this one needs to be scored
                        toScore.Add(rm);
                    }
                }
            }

            // now we score the matchup models
            MarkWinnersInMatchups(toScore);

            AdvanceWinners(toScore, model);

            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));
            int endingRound = model.CheckCurrentRound();
            if (endingRound > startingRound)
            {
                // alert users call for every member in every team.
                model.AlertUsersToNewRound();
            }
        }

        public static void AlertUsersToNewRound(this TournamentModel model)
        {
            int currentRoundNumber = model.CheckCurrentRound();
            // grab a round where the round has a matchup with the current round number
            List<MatchupModel> currentRound = model.Rounds.Where(x => x.First().MatchupRound == currentRoundNumber).First();

            foreach (MatchupModel matchup in currentRound)
            {
                foreach(MatchupEntryModel me in matchup.Entries)
                {
                    foreach(PersonModel p in me.TeamCompeting.TeamMembers)
                    {
                        AlertPersonToNewRound(p, me.TeamCompeting.TeamName, matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault());
                    }
                }
            }
        }
        private static void AlertPersonToNewRound(PersonModel p, string TeamName, MatchupEntryModel competitor)
        {
            if (p.EmailAddress.Length == 0)
            {
                return;
            }

            string to = "";
            string subject = "";
            StringBuilder body = new StringBuilder();

            if (competitor != null)
            {
                subject = $"You have a new matchup with { competitor.TeamCompeting.TeamName } ";
                body.AppendLine("<h1>You have a new matchup</h1>");
                body.Append("<strong>Competitor: </strong>");
                body.AppendLine(competitor.TeamCompeting.TeamName);
                body.AppendLine();
                body.AppendLine();
                body.AppendLine("Have a great time!");
                body.AppendLine("~Tournament Tracker");
            }
            else
            {
                body.AppendLine("You have a bye week this round");
                body.AppendLine("<h1>Enjoy your week off!</h1>");
                body.AppendLine("~Tournament Tracker");
            }

            to = p.EmailAddress;
            EmailLogic.SendEmail(to, subject, body.ToString());
            

        }


        private static int CheckCurrentRound(this TournamentModel model)
        {
            int output = 1;
            foreach (List<MatchupModel> round in model.Rounds)
            {
                // if all matchups in the round have a winner, round is done
                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                } 
                else
                {
                    return output;
                }
            }
            // Tournament is complete
            CompleteTournament(model);

            return output - 1;
            
        }
        private static void CompleteTournament(TournamentModel model)
        {
            GlobalConfig.Connection.CompleteTournament(model);
            // last round is list of matchups with 1 item, the first is only one, the winner is the winner
            TeamModel winners = model.Rounds.Last().First().Winner; // Winner of final matchup
            TeamModel runnerUp = model.Rounds.Last().First().Entries.Where(x => x.TeamCompeting != winners).First().TeamCompeting;
            // TODO - find other runners up if needed.

            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;

            if (model.Prizes.Count > 0)
            {
                // how much money
                decimal totalIncome = model.EnteredTeams.Count * model.EntryFee;

                PrizeModel firstPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
                if (firstPlacePrize != null)
                {
                    winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);
                }

                PrizeModel secondPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();
                if (secondPlacePrize != null)
                {
                    runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);
                }

            }
            // send email to all tournament
            string subject = "";
            StringBuilder body = new StringBuilder();

            subject = $"In the Tournament {model.TournamentName}, { winners.TeamName } has won! ";
            body.AppendLine("<h1>We have WINNER!</h1>");
            body.AppendLine("<p>Congratulations to our winner on a great tournament.</p>");
            body.AppendLine("<br />");

            if (winnerPrize > 0)
            {
                body.AppendLine($"<p>{ winners.TeamName } will recieve ${winnerPrize}</p>");
            }
            if (runnerUpPrize > 0)
            {
                body.AppendLine($"<p>{ runnerUp.TeamName } will recieve ${runnerUpPrize}</p>");
            }
            body.AppendLine("<p>Thanks for a great tournament!</p>");
            body.AppendLine("~Tournament Tracker");

            List<string> bcc = new List<string>();
            foreach(TeamModel t in model.EnteredTeams)
            {
                foreach(PersonModel p in t.TeamMembers)
                {
                    if (p.EmailAddress.Length > 0)
                    {
                        bcc.Add(p.EmailAddress);
                    }
                    
                }
            }
            
            EmailLogic.SendEmail(new List<string>(), bcc, subject, body.ToString());

            // complete tournament
            model.CompleteTournament();
        }
        private static decimal CalculatePrizePayout(this PrizeModel prize, decimal totalIncome)
        {
            decimal output = 0;
            if (prize.PrizeAmount > 0)
            {
                output = prize.PrizeAmount;
            }
            else
            {
                output = Decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
            }
            return output;
        }
        private static void MarkWinnersInMatchups(List<MatchupModel> models)
        {
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel m in models)
            {
                // Checks for Bye Week entry
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;  // break to next iteration, not out.
                }
                // 0 means false, or low score wins
                if (greaterWins == "0")
                {
                    if (m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    } 
                    else if (m.Entries[1].Score < m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    } 
                    else
                    {
                        throw new Exception("we do not allow ties in this application.");
                    }

                }
                else
                {
                    // 1 means true or high score wins
                    if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score > m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("we do not allow ties in this application.");
                    }
                }
            }
        }

        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            foreach (MatchupModel m in models)
            {
                foreach (List<MatchupModel> round in tournament.Rounds)
                {
                    foreach (MatchupModel rm in round)
                    {
                        foreach (MatchupEntryModel me in rm.Entries)
                        {
                            if (me.ParentMatchup != null)
                            {
                                if (me.ParentMatchup.Id == m.Id)
                                {
                                    me.TeamCompeting = m.Winner;
                                    GlobalConfig.Connection.UpdateMatchup(rm);
                                }
                            }
                        }
                    }
                } 
            }
        }
        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            // add rounds to the model
            int round = 2;
            List<MatchupModel> previosRound = model.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in previosRound)
                {
                    // put 2 matchup entries in each matchup
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });
                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currentRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }

                }
                model.Rounds.Add(currentRound);
                previosRound = currentRound;
                currentRound = new List<MatchupModel>();
                round += 1;
                   
            }

        }
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            // Id, List<MatchupEntryModel> Team| score| parent,Team| score| parent, 
            MatchupModel curr = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });
                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();

                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }

            }
            return output;
        }
        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            // Math.Pow(2, rounds);
            int output = 0;
            int totalTeams = 1;
            // get total number of teams needed
            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }
            output = totalTeams - numberOfTeams;
            return output;


        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output+=1; // add a round
                val*=2;    // double val for teams in next val
            }
            return output;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            // cards.OrderBy(a => Guid.NewGuid()).ToList();
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }

    }
}
