using Microsoft.AspNetCore.Mvc;
using Fyra_i_rad.Data;
using Fyra_i_rad.Models;

namespace Fyra_i_rad.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext _context;

        public AdminController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var spelare = _context.Spelare.ToList();
            return View(spelare);
        }

        public IActionResult Delete(int id)
        {
            var spelare = _context.Spelare.FirstOrDefault(s => s.Id == id);
            if (spelare != null)
            {
                _context.Spelare.Remove(spelare);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}


