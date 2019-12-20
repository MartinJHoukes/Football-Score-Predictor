using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballScorePredictor
{
    public class TeamDetails
    {
        public int TeamID  { get; set; }
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
                            TeamName = tm.TeamName,
                            ImagePath = tm.ImagePath

                        }).FirstOrDefault();
            }

            return team;
        }


        public static TeamStanding GetTeamStanding(int teamID)
        {
            var premTeamStandings = (List<TeamStanding>)HttpContext.Current.Session["PREMTeamStandings"];
            var champTeamStandings = (List<TeamStanding>)HttpContext.Current.Session["CHAMPTeamStandings"];

            var teamInfo = premTeamStandings.Where(x => x.ID == teamID).FirstOrDefault();
            if (teamInfo == null)
                teamInfo = champTeamStandings.Where(x => x.ID == teamID).FirstOrDefault();

            return teamInfo;
        }
    }
}