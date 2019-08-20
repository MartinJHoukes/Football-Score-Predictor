using FootballScorePredictor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FootballScorePredictor.Controllers
{
    public class FixturesController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        // Get Next Round Fixtures and Predictions For league (leagueID)
        public ActionResult NextRoundFixtures(int leagueID, int nextRound)
        {
            ViewBag.LeagueID = leagueID;
            ViewBag.Round = nextRound;

            return View();
        }

        //Predictions
        [HttpPost]
        public ActionResult MakePredictions(int leagueID, List<MatchDetails> matchDetails)
        {
            // Make predictions
            var predictions = MatchDetails.MakePredictions(leagueID, matchDetails);

            // Save the predictions to the database
            MatchDetails.SavePredictions(predictions);

            return Json(predictions, JsonRequestBehavior.AllowGet);
        }

        // Get predictions for this round if any
        public JsonResult GetPredictions(int leagueID, int round)
        {
            var predictions = MatchDetails.GetPredicitions(leagueID, round);

            return Json(predictions, JsonRequestBehavior.AllowGet);
        }

        // Save Match Results (if not already saved)
        public ActionResult SaveResults(List<MatchResults> resultsList)
        {
            // Save any Match Results not already saved
            MatchDetails.SaveMatchResults(resultsList);

            return null;
        }


        // Check and Get Forecast Info for this match (if any)
        public ActionResult ForecastInfo(int matchID)
        {


            return null;
        }



    }
}