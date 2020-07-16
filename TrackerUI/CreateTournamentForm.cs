using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequestor, ITeamRequestor
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();

        public CreateTournamentForm()
        {
            InitializeComponent();

            wireUpLists();

        }

        private void wireUpLists()
        {
            selectTeamDropdown.DataSource = null;
            selectTeamDropdown.DataSource = availableTeams;
            selectTeamDropdown.DisplayMember = "TeamName";

            tournamentTeamsListbox.DataSource = null;
            tournamentTeamsListbox.DataSource = selectedTeams;
            tournamentTeamsListbox.DisplayMember = "TeamName";

            prizesListbox.DataSource = null;
            prizesListbox.DataSource = selectedPrizes;
            prizesListbox.DisplayMember = "PlaceName";
        }
        private void selectTeamDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            // take member out of 1st list and move to 2nd
            // returns object so cast to PersonModel
            TeamModel t = (TeamModel)selectTeamDropdown.SelectedItem;

            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);
                wireUpLists();
            }

        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Call the create prize form
            CreatePrizeForm pForm = new CreatePrizeForm(this);
            pForm.Show();
            // return the prize model to current form
            // populate the prize into the selected prize list
        }

        public void prizeComplete(PrizeModel model)
        {
            // Get back from the form a prize model
            // add it to our list
            selectedPrizes.Add(model);
            wireUpLists();
            
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            wireUpLists();
            
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Call the create Team form
            CreateTeamForm tForm = new CreateTeamForm(this);
            tForm.Show();
            // return the team model to current form
            // populate the team into the selected team list
        }

        private void removeSelectedTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListbox.SelectedItem;
            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);
                wireUpLists();
            }
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListbox.SelectedItem;
            if (p != null)
            {
                selectedPrizes.Remove(p);
                wireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // validate Data
            decimal fee = 0;
            bool feeAcceptable = decimal.TryParse(entryFeeText.Text, out fee);

            if (!feeAcceptable)
            {
                MessageBox.Show("Invalid entry fee, please re-enter.",
                    "Invalid Fee",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;  // stop the processing at this point
            }

            // Create Tournament entry
            TournamentModel tm = new TournamentModel();

            tm.TournamentName = tournamentNameText.Text;
            tm.EntryFee = fee;

            tm.Prizes = selectedPrizes;
            tm.EnteredTeams = selectedTeams;

            // 1 TODO - Wire up our matchups
            // 2 order our list randomly, who gets picked when is random, not order you put in.
            // 3 take the list and check if its big enough, otherwise add in byes (skips) (auto wins)
            // 3 2^n power, need 2, 2x2, 2x2x2, 2x2x2x2. etc.
            // 4 create first round of matchups
            // 5 create every round after that
            TournamentLogic.CreateRounds(tm);

            // Create Tournament entry
            // Create all of the prizes entries
            // Create all of the team entries
            // save it
            GlobalConfig.Connection.CreateTournament(tm);

            tm.AlertUsersToNewRound();

            TournamentViewerForm frm = new TournamentViewerForm(tm);
            frm.Show();
            this.Close();


        }
    }
}
