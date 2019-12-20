using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballScorePredictor
{
    public class Predictions
    {
        public int MatchID { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public string Outcome { get; set; }
        public int HomeWinForecast { get; set; } = 0;
        public int AwayWinForecast { get; set; } = 0;
        public int DrawForecast { get; set; } = 0;

    }
}