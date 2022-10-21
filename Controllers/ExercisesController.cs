using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Models;
using WorkoutTracker.Models.ViewModel;
using WorkoutTracker.Services;

namespace WorkoutTracker.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly WorkoutService _workoutService;
        private readonly ExerciseService _exerciseService;
        private readonly CategoryService _categoryService;
        private readonly SetsAndRepsService _setsAndRepsService;
        private readonly MuscleService _muscleService;
        public ExercisesController(WorkoutService workoutService, ExerciseService exerciseService, CategoryService categoryService, SetsAndRepsService setsAndRepsService, MuscleService muscleService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
            _categoryService = categoryService;
            _setsAndRepsService = setsAndRepsService;
            _muscleService = muscleService;

        }
        // GET: ExercisesController
        public async Task<ActionResult> Index()
        {
            ExerciseAndMuscleViewModel exerciseAndMuscleViewModel = new ExerciseAndMuscleViewModel();
            exerciseAndMuscleViewModel.Muscles = await _muscleService.FindAllAsync();
            exerciseAndMuscleViewModel.Exercises = await _exerciseService.FindAllAsync();
            return View(exerciseAndMuscleViewModel);
        }

        // GET: ExercisesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExercisesController/Create
        public async Task<ActionResult> Create()
        {
            CreateExerciseFormViewModel createExerciseFormViewModel = new CreateExerciseFormViewModel();
            createExerciseFormViewModel.Muscles = await _muscleService.FindAllAsync();
            return View(createExerciseFormViewModel);
        }

        // POST: ExercisesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Exercise exercise)
        {
            try
            {
                var muscleIdAndName = Request.Form["createOrSelectedMuscle"];
                if(!string.IsNullOrEmpty(exercise.Name) && !string.IsNullOrEmpty(muscleIdAndName))
                {
                    string[] splittedData = muscleIdAndName.ToString().Split("-");
                    int muscleId;
                    bool hasAnyorIsCreating = int.TryParse(splittedData[0], out muscleId);

                    if (hasAnyorIsCreating)
                    {
                        var muscle = await _muscleService.FindByIdAsync(muscleId);
                        exercise.Muscles.Add(muscle);
                    }
                    else
                    {
                        Muscle muscle = new Muscle { Name = splittedData[1]};
                        await _muscleService.Insert(muscle);
                        exercise.Muscles.Add(muscle);
                    }
                    await _exerciseService.Insert(exercise);
                    return RedirectToAction(nameof(Index));
                }
                return NoContent();
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
