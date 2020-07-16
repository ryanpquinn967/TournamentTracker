using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {   /// <summary>
        /// The unique identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// represents the firstname
        /// </summary>
        public string FirstName { get; set; }
        // represents the last name
        public string LastName { get; set; }
        /// <summary>
        ///  represents the email address for sending tournament details
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// represents cellphone number for text messages
        /// </summary>
        public string CellphoneNumber { get; set; }

        public string FullName 
        {
            get
            {
                return $"{ FirstName } { LastName }";
            }
        }

        public PersonModel()
        {

        }
        public PersonModel(string firstName, string lastName, string emailAddress, string cellphoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            CellphoneNumber = cellphoneNumber;
        }
    }
}
