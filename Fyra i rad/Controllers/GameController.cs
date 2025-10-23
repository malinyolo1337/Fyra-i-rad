
using Microsoft.Data.SqlClient;
using System.Data;
using Fyra_i_rad.Models;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc;
using Fyra_i_rad.Models;

namespace Fyra_i_rad.Controllers
{
    public class GameController : Controller
    {
        private readonly GameMethods _gameMethods;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public GameController(IConfiguration configuration)
        {
            _gameMethods = new GameMethods(configuration);
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult StartaSpel()
        {
            int? spelarID1 = HttpContext.Session.GetInt32("SpelarID1");
            int? spelarID2 = HttpContext.Session.GetInt32("SpelarID2");

            if (spelarID1 == null || spelarID2 == null)
            {
                TempData["Felmeddelande"] = "Du måste logga in två spelare först.";
                return RedirectToAction("Login", "Spelar");
            }

            int spelID = _gameMethods.SkapaNyttSpel();
            _gameMethods.LäggTillSpeldeltagare(spelID, spelarID1.Value, "Röd");
            _gameMethods.LäggTillSpeldeltagare(spelID, spelarID2.Value, "Blå");

            if (!_gameMethods.HarExaktTvåDeltagare(spelID))
            {
                TempData["Felmeddelande"] = "Spelet saknar två deltagare.";
                return RedirectToAction("Login", "Spelar");
            }

            return RedirectToAction("VisaBräde", "Spelrunda", new { spelID });
        }
    }
}




