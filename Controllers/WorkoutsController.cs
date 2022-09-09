using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Services;

namespace WorkoutTracker.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly WorkoutService _workoutService;
        public WorkoutsController(WorkoutService workoutService)
        {
            _workoutService = workoutService;
        }
        // GET: WorkoutsController
        public async Task<ActionResult> Index()
        {
            var workouts = await _workoutService.FindAllAsync();
            return View(workouts);
        }

        // GET: WorkoutsController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if(id != null)
            {
                var workout = await _workoutService.FindByIdAsync((int)id);
                if(workout == null)
                {
                    return View("Error");
                }
                return View(workout);
            }
            return BadRequest();
        }

        // GET: WorkoutsController/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: WorkoutsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
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

        // GET: WorkoutsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View();
        }

        // POST: WorkoutsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
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

        // GET: WorkoutsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View();
        }

        // POST: WorkoutsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
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
