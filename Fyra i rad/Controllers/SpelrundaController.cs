using Fyra_i_rad.Models;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace Fyra_i_rad.Controllers
{
    public class SpelrundaController : Controller
    {
       

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SpelrundaController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VisaBräde(int spelID)
        {
            var bräde = SpelrundaMethods.ByggSpelbräde(_connectionString, spelID);
            int turSpelareID = SpelrundaMethods.VemsTur(_connectionString, spelID);

            var gameMethods = new GameMethods(_configuration);
            var deltagare = gameMethods.HämtaDeltagare(spelID);

            // Skapa färgkarta: SpelarID → färg
            var färgMap = deltagare.ToDictionary(
                d => d.SpelarID,
                d => d.SpelarRoll.ToLower() == "röd" ? "red" :
                     d.SpelarRoll.ToLower() == "blå" ? "blue" : "gray"
            );
            ViewBag.FärgMap = färgMap;

            // Lägg till färgtext (svenska) för varje spelare
            var färgTextMap = deltagare.ToDictionary(
                d => d.SpelarID,
                d => d.SpelarRoll.ToLower() == "röd" ? "röd" :
                     d.SpelarRoll.ToLower() == "blå" ? "blå" : "okänd"
            );
            ViewBag.FärgTextMap = färgTextMap;

            string aktuellFärg = färgMap.ContainsKey(turSpelareID) ? färgMap[turSpelareID] : "okänd";
            string turSpelareNamn = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
            SELECT d.SpelarID, s.Username
            FROM SpelDeltagare d
            JOIN Spelare s ON d.SpelarID = s.SpelarID
            WHERE d.SpelID = @spelID", conn);

                cmd.Parameters.AddWithValue("@spelID", spelID);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int spelareID = reader.GetInt32(0);
                    string username = reader.GetString(1);

                    if (spelareID == turSpelareID)
                    {
                        turSpelareNamn = username;
                        break;
                    }
                }
            }
            string färgText = färgTextMap.ContainsKey(turSpelareID) ? färgTextMap[turSpelareID] : "okänd";
            ViewBag.AktuellSpelare = turSpelareNamn != null
                ? $"{turSpelareNamn} ({färgText})"
                : $"Spelare {turSpelareID} ({färgText})";

            //ViewBag.AktuellSpelare = turSpelareNamn ?? $"Spelare {turSpelareID}";
            //ViewBag.AktuellFärgText = färgTextMap.ContainsKey(turSpelareID) ? färgTextMap[turSpelareID] : "okänd";

           // ViewBag.AktuellSpelare = turSpelareNamn != null
           //? $"{turSpelareNamn} ()"
           //: $"Spelare {turSpelareID}( )";


            //ViewBag.AktuellSpelare = turSpelareNamn != null
            //    ? $"{turSpelareNamn} ({aktuellFärg})"
            //    : $"Spelare {turSpelareID} ({aktuellFärg})";

           

            ViewBag.SpelID = spelID;
            ViewBag.SpeletAvslutat = gameMethods.AvslutatSpel(spelID);
            ViewBag.VinnareID = gameMethods.HämtaVinnare(spelID);

            return View(bräde);
        }


        //public IActionResult VisaBräde(int spelID)
        //{
        //    var bräde = SpelrundaMethods.ByggSpelbräde(_connectionString, spelID);
        //    int turSpelareID = SpelrundaMethods.VemsTur(_connectionString, spelID);

        //    var gameMethods = new GameMethods(_configuration);
        //    var deltagare = gameMethods.HämtaDeltagare(spelID);

        //    // Skapa färgkarta: SpelarID → färg (t.ex. "red", "blue")
        //    var färgMap = deltagare.ToDictionary(
        //        d => d.SpelarID,
        //        d => d.SpelarRoll.ToLower() == "röd" ? "red" :
        //             d.SpelarRoll.ToLower() == "blå" ? "blue" : "gray" // fallback om något är fel
        //    );

        //    // Skicka färgkartan till vyn
        //    ViewBag.FärgMap = färgMap;

        //    // Visa turspelare med färg från roll
        //    string aktuellFärg = färgMap.ContainsKey(turSpelareID) ? färgMap[turSpelareID] : "okänd";
        //    //var namnMap = deltagare.ToDictionary(d => d.SpelarID, d => d.Username);
        //    //ViewBag.NamnMap = namnMap;
        //    using (var conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();
        //        var cmd = new SqlCommand(@"
        //SELECT d.SpelarID, s.Username
        //FROM SpelDeltagare d
        //JOIN Spelare s ON d.SpelarID = s.SpelarID
        //WHERE d.SpelID = @spelID", conn);

        //        cmd.Parameters.AddWithValue("@spelID", spelID);

        //        using var reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            int spelareID = reader.GetInt32(0);
        //            string username = reader.GetString(1);

        //            if (spelareID == turSpelareID)
        //            {
        //                turSpelareNamn = username;
        //                break;
        //            }



        //        //string aktuellNamn = namnMap.ContainsKey(turSpelareID) ? namnMap[turSpelareID] : $"Spelare {turSpelareID}";
        //        //ViewBag.AktuellSpelare = $"{aktuellNamn} ({aktuellFärg})";

        //        ViewBag.AktuellSpelare = $"Spelare {turSpelareNamn} ({aktuellFärg})";

        //        ViewBag.SpelID = spelID;

        //        ViewBag.SpeletAvslutat = gameMethods.AvslutatSpel(spelID);  // bool: true när spelet är avslutat
        //        ViewBag.VinnareID = gameMethods.HämtaVinnare(spelID);  // int? : vinnaren om det finns
        //        string turSpelareNamn = null;



        //        return View(bräde);
        //    }


        //}}


        [HttpPost]
        public IActionResult SkapaDrag(int spelID, int kolumn)
        {
            //int? spelarID = HttpContext.Session.GetInt32("SpelarID");
            int turSpelarID = SpelrundaMethods.VemsTur(_connectionString, spelID);

            if (turSpelarID == null)
            {
                TempData["Felmeddelande"] = "Du måste vara inloggad.";
                return RedirectToAction("VisaBräde", new { spelID });
            }

            if (!SpelrundaMethods.GiltigtDrag(_connectionString, spelID, kolumn, turSpelarID))
            {
                TempData["Felmeddelande"] = "Ogiltigt drag.";
                return RedirectToAction("VisaBräde", new { spelID });
            }

            SpelrundaMethods.SparaDrag(_connectionString, spelID, kolumn, turSpelarID);

            //if (SpelrundaMethods.KontrolleraVinst(_connectionString, spelID, kolumn, turSpelarID))
            //{
            //    var gameMethods = new GameMethods(_configuration);
            //    gameMethods.UppdateraVinnareOchFörlorare(spelID, turSpelarID);

               
            //    //TempData["Vinst"] = "Du vann!";
            //}

            return RedirectToAction("VisaBräde", new { spelID });
        }
        
        [HttpPost]
        public IActionResult Drop(int spelID, int kolumn)
        {
            var spelarID1 = HttpContext.Session.GetInt32("SpelarID1");
            var spelarID2 = HttpContext.Session.GetInt32("SpelarID2");

            if (spelarID1 == null || spelarID2 == null)
            {
                TempData["Felmeddelande"] = "Två spelare måste vara inloggade.";
                return RedirectToAction("Login", "Spelar");
            }

            int turSpelareID = SpelrundaMethods.VemsTur(_connectionString, spelID);

            if (!SpelrundaMethods.GiltigtDrag(_connectionString, spelID, kolumn, turSpelareID))
            {
                TempData["Felmeddelande"] = "Ogiltigt drag";
                return RedirectToAction("VisaBräde", new { spelID });
            }

            SpelrundaMethods.SparaDrag(_connectionString, spelID, kolumn, turSpelareID);

            if (SpelrundaMethods.KontrolleraVinst(_connectionString, spelID, kolumn, turSpelareID))
            {
                var gameMethods = new GameMethods(_configuration);
                gameMethods.UppdateraVinnareOchFörlorare(spelID, turSpelareID);

                string vinnareText = turSpelareID == spelarID1 ? "Röd" : "Blå";
                TempData["Vinst"] = $"Spelare {vinnareText} vann!";
            }

            return RedirectToAction("VisaBräde", new { spelID });
        }
    }
}




