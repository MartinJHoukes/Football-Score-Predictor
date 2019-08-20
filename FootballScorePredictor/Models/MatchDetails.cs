using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballScorePredictor.Models
{
    public class MatchDetails
    {
        public int Round { get; set; }
        public int LeagueID { get; set; }
        public int MatchID { get; set; }
        public int HomeTeamID { get; set; }
        public int AwayTeamID { get; set; }
        public int HomeTeamStanding { get; set; }
        public int AwayTeamStanding { get; set; }
        public int HomeTeamPredictScore { get; set; }
        public int AwayTeamPredictScore { get; set; }
        public DateTime ForecastDate { get; set; }


        public static int FindMinimum(int number1, int number2)
        {
            //3 stored in number1, 5 stored in number2
            int minimum = number2;
            if (number1 < number2) minimum = number1;

            return minimum;
        }

        public static List<MatchDetails> MakePredictions(int leagueID, List<MatchDetails> matchDetails)
        {
            // Get the list of current league standings 
            var teamStandings = new List<TeamStanding>();
            if (leagueID == 2021)
                teamStandings = (List<TeamStanding>)HttpContext.Current.Session["PREMTeamStandings"];
            else
                teamStandings = (List<TeamStanding>)HttpContext.Current.Session["CHAMPTeamStandings"];

            // Make the predictions
            Random rnd = new Random(DateTime.Now.Millisecond);

            foreach (var match in matchDetails)
            {
                var hTeamStanding = teamStandings.Where(x => x.ID == match.HomeTeamID).Select(x => x.Position).FirstOrDefault();
                var aTeamStanding = teamStandings.Where(x => x.ID == match.AwayTeamID).Select(x => x.Position).FirstOrDefault();
                var diff = hTeamStanding - aTeamStanding;
                switch (diff)
                {
                    case int n when (n <= -18):
                        match.HomeTeamPredictScore = rnd.Next(2, 6);
                        match.AwayTeamPredictScore = rnd.Next(2);
                        break;
                    case int n when (n <= -12 && n >= -17):
                        match.HomeTeamPredictScore = rnd.Next(2, 5);
                        match.AwayTeamPredictScore = rnd.Next(3);
                        break;
                    case int n when (n <= -6 && n >= -11):
                        match.HomeTeamPredictScore = rnd.Next(1, 5);
                        match.AwayTeamPredictScore = rnd.Next(3);
                        break;
                    case int n when (n <= 0 && n >= -5):
                        match.HomeTeamPredictScore = rnd.Next(1, 4);
                        match.AwayTeamPredictScore = rnd.Next(3);
                        break;
                    case int n when (n <= 5 && n >= 1):
                        match.HomeTeamPredictScore = rnd.Next(3);
                        match.AwayTeamPredictScore = rnd.Next(3);
                        break;
                    case int n when (n <= 11 && n >= 6):
                        match.HomeTeamPredictScore = rnd.Next(2);
                        match.AwayTeamPredictScore = rnd.Next(1, 4);
                        break;
                    case int n when (n <= 17 && n >= 12):
                        match.HomeTeamPredictScore = rnd.Next(2);
                        match.AwayTeamPredictScore = rnd.Next(1, 4);
                        break;
                    case int n when (n >= 18):
                        match.HomeTeamPredictScore = rnd.Next(2);
                        match.AwayTeamPredictScore = rnd.Next(2, 5);
                        break;
                }
            }

            return matchDetails;
        }

        public static void SavePredictions(List<MatchDetails> matchDetails)
        {
            using (var tdc = new FootballForcastDataContext())
            {
                foreach (var match in matchDetails)
                {
                    var forecast = new Forecast()
                    {
                        Round = match.Round,
                        LeagueID = match.LeagueID,
                        MatchID = match.MatchID,
                        HomeTeamID = match.HomeTeamID,
                        AwayTeamID = match.AwayTeamID,
                        HomeScoreForecast = match.HomeTeamPredictScore,
                        AwayScoreForecast = match.AwayTeamPredictScore,
                        ForecastDate = DateTime.Now
                    };

                    tdc.Forecasts.InsertOnSubmit(forecast);
                }
                tdc.SubmitChanges();
            }
        }

        // Get predicitions for this league / round
        public static List<Predictions> GetPredicitions(int leagueID, int round)
        {
            var predictions = new List<Predictions>();
            using (var tdc = new FootballForcastDataContext())
            {
                var forecasts = tdc.Forecasts.Where(x => x.LeagueID == leagueID && x.Round == round).ToList();
                foreach (var item in forecasts)
                {
                    var prediction = new Predictions()
                    {
                        MatchID = item.MatchID,
                        HomeScore = item.HomeScoreForecast.Value,
                        AwayScore = item.AwayScoreForecast.Value,
                    };



                    // Compare against result (If match played) to get CorrectResult / CorrectScore outcome
                    var result = tdc.Results.Where(x => x.MatchID == item.MatchID).FirstOrDefault();
                    if (result != null)
                    {
                        if (result.HomeScore == item.HomeScoreForecast && result.AwayScore == item.AwayScoreForecast)
                            prediction.Outcome = "CorrectScore";
                        else if ((result.HomeScore == result.AwayScore && item.HomeScoreForecast == item.AwayScoreForecast) ||
                                (result.HomeScore > result.AwayScore && item.HomeScoreForecast > item.AwayScoreForecast) ||
                                (result.HomeScore < result.AwayScore && item.HomeScoreForecast < item.AwayScoreForecast))
                            prediction.Outcome = "CorrectResult";
                        else
                            prediction.Outcome = "Wrong";
                    }

                    predictions.Add(prediction);
                }
            }

            return predictions;
        }

        // Save Match Results
        public static void SaveMatchResults(List<MatchResults> matchResults)
        {
            if (matchResults != null)
            {
                using (var tdc = new FootballForcastDataContext())
                {
                    foreach (var result in matchResults)
                    {
                        if (tdc.Results.Where(x => x.MatchID == result.MatchID).Count() == 0)
                        {
                            // Save Result
                            var mResult = new Result()
                            {
                                MatchID = result.MatchID,
                                HomeTeamID = result.HomeTeamID,
                                AwayTeamID = result.AwayTeamID,
                                HomeScore = result.HomeScore,
                                AwayScore = result.AwayScore
                            };
                            tdc.Results.InsertOnSubmit(mResult);
                        }
                    }
                    tdc.SubmitChanges();
                }
            }
        }
    }
}