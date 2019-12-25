

// GET LEAGUE TABLE STANDINGS ***************************************************************************************************************
function GetTableAPI(league) {

    var url = "https://api.football-data.org/v2/competitions/2021/standings?";
    if (league === "CHAMP") {
        url = "https://api.football-data.org/v2/competitions/2016/standings?";
    }

    $.ajax({
        headers: { 'X-Auth-Token': 'caf25c72c418408da3c9a36e728c5d95' },
        url: url,
        dataType: 'json',
        type: 'GET'
    }).done(function (response) {
        // do something with the response, e.g. isolate the id of a linked resource

        var lowRound = 100;

        // List of object to hold the team standings
        var teamStandingsList = new Array();

        $.each(response.standings[0].table, function (index, table) {

            // Get the lowest round
            if (table.playedGames < lowRound)
                lowRound = table.playedGames;

            var teamImage = tImage[table.team.id];

            //// Save Team Details to the database (Do this only Once at beginning of season)
            //var teamID = table.team.id;
            //var teamName = table.team.name;
            //var tDetails = {};
            //tDetails['TeamID'] = teamID;
            //tDetails['TeamName'] = teamName;
            //tDetails['ImagePath'] = teamImage;
            //var turl = "/Home/SaveTeamDetails";
            //$.post(turl, { team: tDetails });


            // String build each table row
            var teamDetails = tools.getStringBuilder()
                .append('<tr style="background-color: white">')
                .append('<td class="px-1 d-none d-sm-table-cell text-center">' + table.position + '</td>') // Position
                .append('<td class="pr-2 colTeam teamRow" title="Teams Results" onclick="TeamResults(' + table.team.id + ')"><img src=' + teamImage + ' class="teamImage"</img>' + table.team.name + '</td>') // Team Name and Image
                .append('<td class="text-center figCol">' + table.playedGames.toString() + '</td>') // Games Played
                .append('<td class="d-none d-sm-table-cell text-center figCol bg-success text-white">' + table.won.toString() + '</td>') // Games Won
                .append('<td class="d-none d-sm-table-cell text-center figCol bg-warning text-dark">' + table.draw.toString() + '</td>') // Games Drawn
                .append('<td class="d-none d-sm-table-cell text-center figCol bg-danger text-white">' + table.lost.toString() + '</td>') // Games Lost
                .append('<td class="d-none d-sm-table-cell text-center figCol">' + table.goalsFor.toString() + '</td>') // Goals For
                .append('<td class="d-none d-sm-table-cell text-center figCol">' + table.goalsAgainst.toString() + '</td>') // Goals Against
                .append('<td class="d-none d-sm-table-cell text-center figCol">' + table.goalDifference.toString() + '</td>') // Goal Difference
                .append('<td class="text-center font-weight-bold figCol ptsCol">' + table.points.toString() + '</td>') // Total Points
                .append('</tr>')
                .toString();

            $('#tblLeague').append(teamDetails);

            // Populate the team standing array
            var teamStanding = {
                'ID': table.team.id,
                'Position': table.position,
                'TeamName': table.team.name,
                'Played': table.playedGames,
                'Won': table.won,
                'Drawn': table.draw,
                'Lost': table.lost,
                'For': table.goalsFor,
                'Against': table.goalsAgainst,
                'GD': table.goalDifference,
                'Points': table.points,
                'ImagePath': teamImage
            };
            teamStandingsList.push(teamStanding);
        });

        // Send Team Standing to the Controller
        var data = JSON.stringify(teamStandingsList);
        let urlSt = "/Home/SaveCurrentStandings?league=" + league;
        $.ajax({
            type: "POST",
            url: urlSt,
            contentType: 'application/json; charset=utf-8',
            data: data,
            success: function () {
            },
            error: function (xhr, textStatus, error) {
                alert(xhr.responseText);
            }
        });

        $('#nextRound').text(lowRound + 1);
        $('#roundHeader').html("<i class='fa fa-list'></i> Round " + (lowRound + 1).toString() + " Fixtures and Forecasts");

        var lastUpdated = new Date(response.competition.lastUpdated);
        lastUpdated = lastUpdated.toDateString();
        $('#lblLastUpdated').text(lastUpdated);
    });
}
// *******************************************************************************************************************************************

// Get the selected team's results
function TeamResults(teamID) {

    let url = "/Fixtures/TeamResults?teamID=" + teamID;
    window.location.href = url;
}

// String Builder
var tools = {
    getStringBuilder: function () {
        var data = [];
        var counter = 0;
        return {
            // adds string s to the stringbuilder
            append: function (s) { data[counter++] = s; return this; },
            // removes j elements starting at i, or 1 if j is omitted
            remove: function (i, j) { data.splice(i, j || 1); return this; },
            // inserts string s at i
            insert: function (i, s) { data.splice(i, 0, s); return this; },
            // builds the string
            toString: function (s) { return data.join(s || ""); }
        };
    }
};
// *************************************************************************************************************************************************

// Month
var month = new Array();
month[0] = "January";
month[1] = "February";
month[2] = "March";
month[3] = "April";
month[4] = "May";
month[5] = "June";
month[6] = "July";
month[7] = "August";
month[8] = "September";
month[9] = "October";
month[10] = "November";
month[11] = "December";

// Day
var day = new Array();
day[0] = "Sunday";
day[1] = "Monday";
day[2] = "Tuesday";
day[3] = "Wednesday";
day[4] = "Thursday";
day[5] = "Friday";
day[6] = "Saturday";

// Team Image
var tImage = new Array();
tImage[64] = "https://upload.wikimedia.org/wikipedia/de/0/0a/FC_Liverpool.svg";
tImage[65] = "https://upload.wikimedia.org/wikipedia/en/e/eb/Manchester_City_FC_badge.svg";
tImage[73] = "https://upload.wikimedia.org/wikipedia/de/b/b4/Tottenham_Hotspur.svg";
tImage[61] = "https://upload.wikimedia.org/wikipedia/de/5/5c/Chelsea_crest.svg";
tImage[57] = "https://upload.wikimedia.org/wikipedia/en/5/53/Arsenal_FC.svg";
tImage[66] = "https://upload.wikimedia.org/wikipedia/de/d/da/Manchester_United_FC.svg";
tImage[338] = "https://upload.wikimedia.org/wikipedia/en/6/63/Leicester02.png";
tImage[346] = "https://upload.wikimedia.org/wikipedia/en/e/e2/Watford.svg";
tImage[76] = "https://upload.wikimedia.org/wikipedia/en/f/fc/Wolverhampton_Wanderers.svg";
tImage[563] = "https://upload.wikimedia.org/wikipedia/de/e/e0/West_Ham_United_FC.svg";
tImage[62] = "https://upload.wikimedia.org/wikipedia/de/f/f9/Everton_FC.svg";
tImage[1044] = "https://upload.wikimedia.org/wikipedia/de/4/41/Afc_bournemouth.svg";
tImage[397] = "https://upload.wikimedia.org/wikipedia/en/f/fd/Brighton_%26_Hove_Albion_logo.svg";
tImage[354] = "https://upload.wikimedia.org/wikipedia/de/b/bf/Crystal_Palace_F.C._logo_%282013%29.png";
tImage[67] = "https://upload.wikimedia.org/wikipedia/de/5/56/Newcastle_United_Logo.svg";
tImage[328] = "https://upload.wikimedia.org/wikipedia/en/0/02/Burnley_FC_badge.png";
tImage[715] = "https://upload.wikimedia.org/wikipedia/en/3/3c/Cardiff_City_crest.svg";
tImage[340] = "https://upload.wikimedia.org/wikipedia/de/c/c9/FC_Southampton.svg";
tImage[63] = "https://upload.wikimedia.org/wikipedia/de/a/a8/Fulham_fc.svg";
tImage[394] = "https://upload.wikimedia.org/wikipedia/en/5/5a/Huddersfield_Town_A.F.C._logo.svg";
tImage[341] = "https://upload.wikimedia.org/wikipedia/en/0/05/Leeds_United_Logo.png";
tImage[68] = "https://upload.wikimedia.org/wikipedia/en/8/8c/Norwich_City.svg";
tImage[356] = "https://upload.wikimedia.org/wikipedia/en/9/9c/Sheffield_United_FC_logo.svg";
tImage[74] = "https://upload.wikimedia.org/wikipedia/en/8/8b/West_Bromwich_Albion.svg";
tImage[343] = "https://upload.wikimedia.org/wikipedia/en/2/2c/Middlesbrough_FC_crest.svg";
tImage[342] = "https://upload.wikimedia.org/wikipedia/en/4/4a/Derby_County_crest.svg";
tImage[351] = "https://upload.wikimedia.org/wikipedia/en/d/d2/Nottingham_Forest_logo.svg";
tImage[332] = "https://upload.wikimedia.org/wikipedia/en/6/68/Birmingham_City_FC_logo.svg";
tImage[69] = "https://upload.wikimedia.org/wikipedia/de/d/d4/Queens_Park_Rangers.svg";
tImage[58] = "https://upload.wikimedia.org/wikipedia/de/9/9f/Aston_Villa_logo.svg";
tImage[387] = "https://upload.wikimedia.org/wikipedia/en/1/1f/Bristol_City_F.C._logo.png";
tImage[72] = "https://upload.wikimedia.org/wikipedia/de/a/ab/Swansea_City_Logo.svg";
tImage[322] = "https://upload.wikimedia.org/wikipedia/en/2/20/Hull_City_Crest_2014.svg";
tImage[70] = "https://upload.wikimedia.org/wikipedia/de/a/a3/Stoke_City.svg";
tImage[59] = "https://upload.wikimedia.org/wikipedia/en/0/0f/Blackburn_Rovers.svg";
tImage[345] = "https://upload.wikimedia.org/wikipedia/en/8/88/Sheffield_Wednesday_badge.svg";
tImage[1081] = "https://upload.wikimedia.org/wikipedia/en/2/21/PNE_FC.png";
tImage[402] = "https://upload.wikimedia.org/wikipedia/en/2/2a/Brentford_FC_crest.svg";
tImage[384] = "https://upload.wikimedia.org/wikipedia/en/7/71/Millwall_FC_logo.png";
tImage[75] = "https://upload.wikimedia.org/wikipedia/en/4/43/Wigan_Athletic.svg";
tImage[385] = "https://upload.wikimedia.org/wikipedia/en/c/c0/Rotherham_United_FC.svg";
tImage[60] = "https://upload.wikimedia.org/wikipedia/en/8/82/Bolton_Wanderers_FC_logo.svg";
tImage[355] = "https://upload.wikimedia.org/wikipedia/en/1/11/Reading_FC.svg";
tImage[349] = "https://upload.wikimedia.org/wikipedia/en/4/43/Ipswich_Town.svg";
tImage[348] = "https://upload.wikimedia.org/wikipedia/en/a/a8/Charlton_Athletic_F.C._logo.png";
tImage[357] = "https://upload.wikimedia.org/wikipedia/en/thumb/c/c9/Barnsley_FC.svg/200px-Barnsley_FC.svg.png";
tImage[389] = "https://upload.wikimedia.org/wikipedia/en/thumb/8/8b/LutonTownFC2009.png/150px-LutonTownFC2009.png";
tImage[394] = "https://upload.wikimedia.org/wikipedia/en/thumb/7/7d/Huddersfield_Town_A.F.C._logo.png/150px-Huddersfield_Town_A.F.C._logo.png";

function checkNull(val) {
    if (val === null)
        return '';
    else
        return val.toString();
}

function Abbreviate(team) {
    return team.replace("AFC", "").replace("FC", "").replace("Albion", "").replace("United", "Utd").replace("Wanderers", "").replace("Wolverhampton", "Wolves")
        .replace("Manchester", "Man").replace("Middlesbrough", "Middles'bro");
}
