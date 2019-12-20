using FootballScorePredictor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FootballScorePredictor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PremierLeague()
        {
            return View();
        }

        public ActionResult Championship()
        {
            return View();
        }

        public ActionResult SaveCurrentStandings(string league, List<TeamStanding> teamStandings)
        {
            Session[league + "TeamStandings"] = teamStandings.ToList<TeamStanding>();

            return null;
        }

        public ActionResult SaveTeamDetails(TeamDetails team)
        {
            TeamDetails.SaveTeamDetails(team);

            return null;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
        
            return View();
        }

        public JsonResult GetAllResults()
        {
            var allResults = MatchResults.GetAllResults();

            return Json(allResults, JsonRequestBehavior.AllowGet);


        }

    }
}