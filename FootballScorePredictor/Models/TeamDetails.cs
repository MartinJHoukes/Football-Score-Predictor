using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballScorePredictor
{
    public class TeamDetails
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string ImagePath { get; set; }


        public static void SaveTeamDetails(TeamDetails team)
        {
            using (var db = new FootballForcastDataContext())
            {
                var fTeam = db.Teams.Where(x => x.TeamID == team.TeamID).Count();
                if (fTeam == 0)
                {
                    var nTeam = new Team()
                    {
                        TeamID = team.TeamID,
                        TeamName = team.TeamName,
                        ImagePath = team.ImagePath
                    };
                    db.Teams.InsertOnSubmit(nTeam);
                    db.SubmitChanges();
                }
            }
        }

        public static TeamDetails GetTeamDetails(int teamID)
        {
            var team = new TeamDetails();
            using (var db = new FootballForcastDataContext())
            {
                team = (from tm in db.Teams
                        where tm.TeamID == teamID
                        select new TeamDetails
                        {
                            TeamID = teamID,
                            TeamName = TeamNameAbbreviate(tm.TeamName),
                            ImagePath = tm.ImagePath

                        }).FirstOrDefault();
            }

            return team;
        }

        public static TeamStanding GetTeamStanding(int teamID)
        {
            var premTeamStandings = new List<TeamStanding>();
            var champTeamStandings = new List<TeamStanding>();

            if (HttpContext.Current.Session["PREMTeamStandings"] != null)
                premTeamStandings = (List<TeamStanding>)HttpContext.Current.Session["PREMTeamStandings"];
            if (HttpContext.Current.Session["CHAMPTeamStandings"] != null)
                champTeamStandings = (List<TeamStanding>)HttpContext.Current.Session["CHAMPTeamStandings"];

            var teamStandings = premTeamStandings.Concat(champTeamStandings).ToList();
            var teamInfo = teamStandings.Where(x => x.ID == teamID).FirstOrDefault();

            return teamInfo;
        }

        private static string TeamNameAbbreviate(string teamName)
        {
            return teamName.Replace("AFC", "").Replace("FC", "").Replace("Albion", "").Replace("United", "Utd").Replace("Wanderers", "").Replace("Wolverhampton", "Wolves")
                .Replace("Manchester", "Man").Replace("Middlesbrough", "Middles'bro");
        }
    }
}