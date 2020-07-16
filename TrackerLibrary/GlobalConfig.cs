using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {

        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PeopleModels.csv";
        public const string TeamsFile = "TeamModels.csv";
        public const string TournamentFile = "TournamentModels.csv";
        public const string MatchupFile = "MatchupModels.csv";
        public const string MatchupEntryFile = "MatchupEntryModels.csv";
        /// <summary>
        /// Connections is a List that holds anything implementing the IDataConnection
        /// </summary>
        public static IDataConnection Connection { get; private set; }

        public static void InitializeConnections(DatabaseType db)
        {

            if (db == DatabaseType.Sql)
            {

                SqlConnector sql = new SqlConnector();
                Connection = sql;

            } 
            else if (db == DatabaseType.TextFile)
            {
                TextConnector textDB = new TextConnector();
                Connection = textDB;

            }
        }

        public static string CnnString(string name)
        {
            // goto app.config and get connection string
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;

        }

        public static string AppKeyLookup(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

    }
}
