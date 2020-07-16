using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class PrizeModel
    {
        /// <summary>
        /// the unique identifer
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// represents the place number in the tournament
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        ///  represents what we refer to the place number as, runner up as example for 2
        /// </summary>
        public string PlaceName { get; set; }
        /// <summary>
        /// a dollar amount for a prize
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        ///  prize that may be represented as a percent of the total, winner gets 50% etc.
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel()
        {

        }

        public PrizeModel(string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;
            
            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;

        }

    }
}
