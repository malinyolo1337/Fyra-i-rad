using FyraIRad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FyraIRad.Controllers
{
    public class SpelarController : Controller
    {
        private readonly IConfiguration _configuration;

        public SpelarController(IConfiguration configuration)
        {
            _configuration = configuration;
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

        [HttpPost]
        public IActionResult Login(string username, string password)
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

        public IActionResult Profile()
        {
            string? username = HttpContext.Session.GetString("Username");
            int? spelarID = HttpContext.Session.GetInt32("SpelarID");

            if (username == null || spelarID == null)
                return RedirectToAction("Login");

            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.GetSpelarById(spelarID.Value);

            ViewBag.Username = spelar.Username;
            ViewBag.Markör = spelar.Markör;
            ViewBag.Vinster = spelar.AntalVinster;
            ViewBag.Förluster = spelar.AntalFörluster;

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult SelectSpelar()
        {
            var spelarMethods = new SpelarMethods(_configuration);
            string error;
            var spelarList = spelarMethods.GetSpelarModelList(out error);

            ViewBag.Error = error;
            return View(spelarList);
        }

        [HttpGet]
        public IActionResult EditSpelar(int spelarID)
        {
            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.GetSpelarById(spelarID);
            return View(spelar);
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
        public IActionResult DeleteSpelar(int spelarID)
        {
            var spelarMethods = new SpelarMethods(_configuration);
            var spelar = spelarMethods.GetSpelarById(spelarID);
            return View(spelar);
        }

        [HttpPost]
        public IActionResult DeleteSpelarConfirmed(int spelarID)
        {
            var spelarMethods = new SpelarMethods(_configuration);
            string error;
            spelarMethods.DeleteSpelar(spelarID, out error);

            TempData["Error"] = error;
            return RedirectToAction("SelectSpelar");
        }
    }
}
