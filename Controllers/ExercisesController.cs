using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutTracker.Controllers
{
    public class ExercisesController : Controller
    {
        // GET: ExercisesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ExercisesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExercisesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExercisesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExercisesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ExercisesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExercisesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExercisesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
