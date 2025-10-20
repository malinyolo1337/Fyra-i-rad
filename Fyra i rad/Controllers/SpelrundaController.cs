using Microsoft.AspNetCore.Mvc;
using Fyra_i_rad.Models;
using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace Fyra_i_rad.Controllers
{
    public class SpelrundaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        private readonly string connectionString;

        public SpelrundaController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public IActionResult VisaBräde(int spelID)
        {
            var bräde = SpelrundaMethods.ByggSpelbräde(connectionString, spelID);
            ViewBag.SpelID = spelID;
            return View(bräde);
        }

        [HttpPost]
        public IActionResult SkapaDrag(int spelID, int kolumn)
        {
            int? spelarID = HttpContext.Session.GetInt32("SpelarID");
            if (spelarID == null)
            {
                TempData["Felmeddelande"] = "Du måste vara inloggad.";
                return RedirectToAction("VisaBräde", new { spelID });
            }

            if (!SpelrundaMethods.ÄrDragGiltigt(connectionString, spelID, kolumn, spelarID.Value))
            {
                TempData["Felmeddelande"] = "Ogiltigt drag.";
                return RedirectToAction("VisaBräde", new { spelID });
            }

            SpelrundaMethods.SparaDrag(connectionString, spelID, kolumn, spelarID.Value);

            if (SpelrundaMethods.KontrolleraVinst(connectionString, spelID, kolumn, spelarID.Value))
            {
                SpelrundaMethods.UppdateraSpelTillVinst(connectionString, spelID, spelarID.Value);
                TempData["Vinstmeddelande"] = "Du vann!";
            }

            return RedirectToAction("VisaBräde", new { spelID });
        }
        [HttpPost]
        public IActionResult Drop(int kolumn)
        {
            int spelID = HttpContext.Session.GetInt32("SpelID") ?? 0;
            int spelarID = HttpContext.Session.GetInt32("SpelarID") ?? 0;

            if (SpelrundaMethods.ÄrDragGiltigt(connectionString, spelID, kolumn, spelarID))
            {
                SpelrundaMethods.SparaDrag(connectionString, spelID, kolumn, spelarID);

                if (SpelrundaMethods.KontrolleraVinst(connectionString, spelID, kolumn, spelarID))
                {
                    SpelrundaMethods.UppdateraSpelTillVinst(connectionString, spelID, spelarID);
                    TempData["Message"] = $"Spelare {spelarID} vann!";
                }
            }

            return RedirectToAction("Index");
        }



    }
}



