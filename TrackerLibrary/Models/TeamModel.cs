using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        /// <summary>
        /// unique team identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// a list of persons representing the members of the team
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();  // new in c# 6
        /// <summary>
        /// represents the name of the team
        /// </summary>
        public string TeamName { get; set; }




    }
}
