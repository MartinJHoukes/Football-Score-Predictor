using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballScorePredictor
{
    public class MatchResults
    {
        public int MatchID { get; set; }
        public string MatchDate { get; set; }
        public string MatchTime { get; set; }
        public int HomeTeamID { get; set; }
        public string HomeTeam { get; set; }
        public string HomeTeamImage { get; set; }
        public int AwayTeamID { get; set; }
        public string AwayTeam { get; set; }
        public string AwayTeamImage { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int HomeTeamForecast { get; set; }
        public int AwayTeamForecast { get; set; }
        public DateTime MatchPlayed { get; set; }
        public string Points { get; set; }


        // Get Team Results 
        public static List<MatchResults> TeamResults(int teamID)
        {
            var listMatches = new List<MatchResults>();
            using (var db = new FootballForcastDataContext())
            {
                listMatches = (from rs in db.Results
                               where rs.HomeTeamID == teamID || rs.AwayTeamID == teamID
                               select new MatchResults
                               {
                                   MatchID = rs.MatchID,
                                   MatchDate = rs.MatchDate,
                                   MatchTime = rs.MatchTime,
                                   HomeScore = rs.HomeScore,
                                   AwayScore = rs.AwayScore,
                                   HomeTeam = TeamDetails.GetTeamDetails(rs.HomeTeamID).TeamName,
                                   HomeTeamImage = TeamDetails.GetTeamDetails(rs.HomeTeamID).ImagePath,
                                   AwayTeamID = rs.AwayTeamID,
                                   HomeTeamID = rs.HomeTeamID,
                                   AwayTeam = TeamDetails.GetTeamDetails(rs.AwayTeamID).TeamName,
                                   AwayTeamImage = TeamDetails.GetTeamDetails(rs.AwayTeamID).ImagePath,
                                   HomeTeamForecast = rs.Forecast.HomeScoreForecast.Value,
                                   AwayTeamForecast = rs.Forecast.AwayScoreForecast.Value,
                                   MatchPlayed= DateTime.Parse(rs.MatchDate),
                                   Points = GetMatchPoints(rs, teamID)
                               }).ToList();
            }

            return listMatches.OrderByDescending(x => x.MatchPlayed).ToList();
        }
        public static List<MatchResults> GetAllResults()
        {
            var allResults = new List<MatchResults>();
            using (var db = new FootballForcastDataContext())
            {
                allResults = (from ms in db.Results
                              select new MatchResults
                              {
                                  MatchID = ms.MatchID,
                                  MatchDate = ms.MatchDate
                              }).ToList();              
            }

            return allResults;
        }

        private static string GetMatchPoints(Result matchResult, int teamID)
        {
            string points = "";
            if (matchResult.HomeScore == matchResult.AwayScore)
                points = "1 Pt";
            else if ((matchResult.HomeScore > matchResult.AwayScore && teamID == matchResult.HomeTeamID) ||
                     (matchResult.AwayScore > matchResult.HomeScore && teamID == matchResult.AwayTeamID))
                points = "3 Pts";
            else
                points = "0 Pts";

            return points;
        }
    } 
}