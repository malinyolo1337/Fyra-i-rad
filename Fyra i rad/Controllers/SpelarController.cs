using FyraIRad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace FyraIRad.Controllers
{
    public class SpelarController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SpelarController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult InsertSpelar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoginProfil()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult LoginProfil(string username, string password)
        {
            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.Login(username, password);

            if (spelar != null)
            {
                HttpContext.Session.SetString("Username", spelar.Username);
                HttpContext.Session.SetInt32("SpelarID", spelar.SpelarID);
                return RedirectToAction("Profile");
            }

            ViewBag.Error = "Fel användarnamn eller lösenord";
            return View();
        }



        [HttpPost]
        public IActionResult LoginTvåSpelare(string username1, string password1, string username2, string password2)
        {
            var spelarMethods = new SpelarMethods(_configuration);
            var spelar1 = spelarMethods.Login(username1, password1);
            var spelar2 = spelarMethods.Login(username2, password2);

            if (spelar1 == null || spelar2 == null || spelar1.SpelarID == spelar2.SpelarID)
            {
                ViewBag.Error = "Felaktiga uppgifter eller samma spelare.";
                return View();
            }

            HttpContext.Session.SetInt32("SpelarID1", spelar1.SpelarID);
            HttpContext.Session.SetInt32("SpelarID2", spelar2.SpelarID);

            return RedirectToAction("StartaSpel", "Game");
        }

        private void LäggTillSpeldeltagare(int spelID, int spelarID, string spelarRoll)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            var cmd = new SqlCommand("INSERT INTO Speldeltagare (SpelID, SpelarID, SpelarRoll) VALUES (@spelID, @spelarID, @spelarRoll)", conn);
            cmd.Parameters.AddWithValue("@spelID", spelID);
            cmd.Parameters.AddWithValue("@spelarID", spelarID);
            cmd.Parameters.AddWithValue("@spelarRoll", spelarRoll);
            cmd.ExecuteNonQuery();
        }



        [HttpPost]
        public IActionResult InsertSpelar(SpelarModel spelar)
        {
            if (!ModelState.IsValid)
                return View(spelar);

            var spelarMethods = new SpelarMethods(_configuration);
            string errormsg;
            int i = spelarMethods.InsertSpelar(spelar, out errormsg);

            if (i == 0)
            {
                ViewBag.Message = "Registrering misslyckades: " + errormsg;
                return View(spelar);
            }

            TempData["Success"] = "Registrering lyckades!";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Message = TempData["Success"];
            return View();
        }

 [HttpGet]
        public IActionResult Profile()
        {
            string? username = HttpContext.Session.GetString("Username");
            int? spelarID = HttpContext.Session.GetInt32("SpelarID");

            if (username == null || spelarID == null)
            {
                return RedirectToAction("LoginProfil");
            }

            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.GetSpelarById(spelarID.Value);

            ViewBag.Username = spelar.Username;
            ViewBag.Vinster = spelar.AntalVinster;
            ViewBag.Förluster = spelar.AntalFörluster;

            return View();
        }
        

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginProfil");
        }

        public IActionResult SelectSpelar()
        {
            var spelarMethods = new SpelarMethods(_configuration);
            string error;
          
            var spelarList = spelarMethods.GetSpelarModelList(out error)
            .OrderByDescending(s => s.AntalVinster)
            .ToList();


            ViewBag.Error = error;
            return View(spelarList);
        }



        [HttpPost]
        public IActionResult EditSpelar(SpelarModel spelar)
        {
            if (!ModelState.IsValid)
                return View(spelar);

            var spelarMethods = new SpelarMethods(_configuration);
            string error;
            spelarMethods.UpdateSpelar(spelar, out error);

            TempData["Error"] = error;
            return RedirectToAction("SelectSpelar");
        }

        

        [HttpGet]
        public IActionResult EditSpelar()
        {
            int? spelarID = HttpContext.Session.GetInt32("SpelarID");
            if (spelarID == null) return RedirectToAction("Login");

            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.GetSpelarById(spelarID.Value);
            return View(spelar);
        }

        [HttpGet]
        public IActionResult DeleteSpelar()
        {
            int? spelarID = HttpContext.Session.GetInt32("SpelarID");
            if (spelarID == null) return RedirectToAction("Login");

            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.GetSpelarById(spelarID.Value);
            return View(spelar);
        }

        [HttpPost]
        public IActionResult DeleteSpelarConfirmed()
        {
            int? spelarID = HttpContext.Session.GetInt32("SpelarID");
            if (spelarID == null) return RedirectToAction("Login");

            var spelarMethods = new SpelarMethods(_configuration);
            string error;
            spelarMethods.DeleteSpelar(spelarID.Value, out error);

            HttpContext.Session.Clear(); // logga ut efter radering
            TempData["Error"] = error;
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult LoginRedigera()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginRedigera(string username, string password)
        {
            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.Login(username, password);

            if (spelar != null)
            {
                HttpContext.Session.SetString("Username", spelar.Username);
                HttpContext.Session.SetInt32("SpelarID", spelar.SpelarID);
                return RedirectToAction("EditSpelar");
            }

            ViewBag.Error = "Fel användarnamn eller lösenord";
            return View();
        }


    }
}
