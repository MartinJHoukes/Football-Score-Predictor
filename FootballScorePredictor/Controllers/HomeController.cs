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

        public ActionResult Championship()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            var minNumber = MatchDetails.FindMinimum(16, 2);

            return View();
        }

        public ActionResult SaveCurrentStandings(string league, List<TeamStanding> teamStandings)
        {
            Session[league + "TeamStandings"] = teamStandings.ToList<TeamStanding>();

            return null;
        }
    }
}