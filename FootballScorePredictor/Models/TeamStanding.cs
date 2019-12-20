using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballScorePredictor
{
    public class TeamStanding
    {
        public int ID { get; set; }
        public int Position { get; set; }
        public string TeamName { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int For { get; set; }
        public int Against { get; set; }
        public int GD { get; set; }
        public int Points { get; set; }
        public string ImagePath{ get; set; }
        public List<MatchResults> Results { get; set; }

    }
}