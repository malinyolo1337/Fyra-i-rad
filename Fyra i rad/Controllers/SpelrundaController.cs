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
            int? spelarID1 = HttpContext.Session.GetInt32("SpelarID1");
            int? spelarID2 = HttpContext.Session.GetInt32("SpelarID2");


            ViewBag.AktuellSpelare = turSpelareID == spelarID1 ? "1 (Röd)" : "2 (Blå)";
            ViewBag.SpelID = spelID;

            return View(bräde);
          
        }


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

            if (SpelrundaMethods.KontrolleraVinst(_connectionString, spelID, kolumn, turSpelarID))
            {
                var gameMethods = new GameMethods(_configuration);
                gameMethods.UppdateraSpelTillVinst(spelID, turSpelarID);

               
                TempData["Vinstmeddelande"] = "Du vann!";
            }

            return RedirectToAction("VisaBräde", new { spelID });
        }
        
        //    }[HttpPost]
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
                TempData["Felmeddelande"] = "Ogiltigt drag – kanske är kolumnen full eller inte din tur.";
                return RedirectToAction("VisaBräde", new { spelID });
            }

            SpelrundaMethods.SparaDrag(_connectionString, spelID, kolumn, turSpelareID);

            if (SpelrundaMethods.KontrolleraVinst(_connectionString, spelID, kolumn, turSpelareID))
            {
                var gameMethods = new GameMethods(_configuration);
                gameMethods.UppdateraSpelTillVinst(spelID, turSpelareID);

                string vinnareText = turSpelareID == spelarID1 ? "1 (Röd)" : "2 (Blå)";
                TempData["Vinst"] = $"Spelare {vinnareText} vann!";
            }

            return RedirectToAction("VisaBräde", new { spelID });
        }
    }
}




