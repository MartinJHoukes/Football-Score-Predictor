using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballScorePredictor
{
    public class MatchResults
    {
        public int MatchID { get; set; }
        public int HomeTeamID { get; set; }
        public int AwayTeamID { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

    }
}