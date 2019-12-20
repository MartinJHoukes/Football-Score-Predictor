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
        public int HomeWinForecast { get; set; }
        public int AwayWinForecast { get; set; }
        public int DrawForecast { get; set; }
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

            foreach (var match in matchDetails)
            {
                var hTeamStanding = teamStandings.Where(x => x.ID == match.HomeTeamID).Select(x => x.Position).FirstOrDefault();
                var aTeamStanding = teamStandings.Where(x => x.ID == match.AwayTeamID).Select(x => x.Position).FirstOrDefault();
                var diff = hTeamStanding - aTeamStanding;

                // Run the forecast 100 times to find forecast precentages 
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                var listForecasts = new List<MatchForecast>();
                for (int i = 0; i < 100; i++)
                {
                    var matchForecast = ForecastMatch(diff, rnd);
                    if (matchForecast.HomeScore > matchForecast.AwayScore)
                    {
                        match.HomeWinForecast += 1;
                        matchForecast.ForecastResult = "Home";
                    }
                    else if (matchForecast.HomeScore < matchForecast.AwayScore)
                    {
                        match.AwayWinForecast += 1;
                        matchForecast.ForecastResult = "Away";
                    }
                    else
                    {
                        match.DrawForecast += 1;
                        matchForecast.ForecastResult = "Draw";
                    }
                    listForecasts.Add(matchForecast);
                }       

                // Get forecast outcome
                MatchForecast mForecast;
                if (match.HomeWinForecast > match.AwayWinForecast && match.HomeWinForecast > match.DrawForecast)
                    mForecast = listForecasts.Where(x => x.ForecastResult == "Home").Last();           
                else if (match.AwayWinForecast > match.HomeWinForecast && match.AwayWinForecast > match.DrawForecast)
                    mForecast = listForecasts.Where(x => x.ForecastResult == "Away").Last();
                else
                    mForecast = listForecasts.Where(x => x.ForecastResult == "Draw").Last();

                match.HomeTeamPredictScore = mForecast.HomeScore;
                match.AwayTeamPredictScore = mForecast.AwayScore;

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
                        ForecastDate = DateTime.Now,
                        HomeWinForecast = match.HomeWinForecast,
                        AwayWinForecast = match.AwayWinForecast,
                        DrawForecast = match.DrawForecast
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
                    if (item.HomeWinForecast == null)
                        item.HomeWinForecast = 0;
                    if (item.AwayWinForecast == null)
                        item.AwayWinForecast = 0;
                    if (item.DrawForecast == null)
                        item.DrawForecast = 0;

                    var prediction = new Predictions()
                    {
                        MatchID = item.MatchID,
                        HomeScore = item.HomeScoreForecast.Value,
                        AwayScore = item.AwayScoreForecast.Value,
                        HomeWinForecast = item.HomeWinForecast.Value,
                        AwayWinForecast = item.AwayWinForecast.Value,
                        DrawForecast = item.DrawForecast.Value
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
                                AwayScore = result.AwayScore,
                                MatchDate = result.MatchDate,
                                MatchTime = result.MatchTime
                            };
                            tdc.Results.InsertOnSubmit(mResult);
                        }
                    }
                    tdc.SubmitChanges();
                }
            }
        }

        // Forecast the Match
        private static MatchForecast ForecastMatch(int diff, Random rnd)
        {
            // Make the predictions         
            var mForecast = new MatchForecast();

            switch (diff)
            {
                case int n when (n <= -18):
                    mForecast.HomeScore = rnd.Next(1, 6); // 1 to 6 goals
                    mForecast.AwayScore = rnd.Next(3); // Max 2 goals
                    break;
                case int n when (n <= -12 && n >= -17):
                    mForecast.HomeScore = rnd.Next(1, 5); // 1 to 5 goals
                    mForecast.AwayScore = rnd.Next(3); // Max 2 goals
                    break;
                case int n when (n <= -6 && n >= -11):
                    mForecast.HomeScore = rnd.Next(1, 4); // 1 to 4 goals
                    mForecast.AwayScore = rnd.Next(3); // Max 2 goals
                    break;
                case int n when (n <= 0 && n >= -5):
                    mForecast.HomeScore = rnd.Next(3); // Max 2 goals 
                    mForecast.AwayScore = rnd.Next(3); // Max 2 goals
                    break;
                case int n when (n <= 5 && n >= 1):
                    mForecast.HomeScore = rnd.Next(3); // Max 2 goals
                    mForecast.AwayScore = rnd.Next(3); // max 2 goals
                    break;
                case int n when (n <= 11 && n >= 6):
                    mForecast.HomeScore = rnd.Next(3); // Max 2 goals
                    mForecast.AwayScore = rnd.Next(1, 4); // 1 to 4 goals
                    break;
                case int n when (n <= 17 && n >= 12):
                    mForecast.HomeScore = rnd.Next(3); // Max 2 goals 
                    mForecast.AwayScore = rnd.Next(1, 4); // 1 to 4 goals
                    break;
                case int n when (n >= 18):
                    mForecast.HomeScore = rnd.Next(3); // Max 2 goals
                    mForecast.AwayScore = rnd.Next(1, 5); // 1 to 5 goals
                    break;
            }

            return mForecast;
        }

    }
}