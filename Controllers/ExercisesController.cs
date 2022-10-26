using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Models;
using WorkoutTracker.Models.ViewModel;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Exceptions;

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
                if (!string.IsNullOrEmpty(exercise.Name) && !string.IsNullOrEmpty(muscleIdAndName))
                {
                    string[] splittedData = muscleIdAndName.ToString().Split("-");
                    int muscleId;
                    bool hasAnyIdOrIsCreating = int.TryParse(splittedData[0], out muscleId);

                    if (hasAnyIdOrIsCreating)
                    {
                        var muscle = await _muscleService.FindByIdAsync(muscleId);
                        exercise.Muscles.Add(muscle);
                    }
                    else
                    {
                        Muscle muscle = new Muscle { Name = splittedData[1] };
                        await _muscleService.InsertAsync(muscle);
                        exercise.Muscles.Add(muscle);
                    }
                    await _exerciseService.InsertAsync(exercise);
                    return RedirectToAction(nameof(Index));
                }
                return NoContent();
            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Error), new {message = ex.Message, solution = ""});
            }
        }

        // GET: ExercisesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id != 0)
            {
                var exercise = await _exerciseService.FindByIdAsync(id);
                if (exercise != null)
                {
                    CreateExerciseFormViewModel createExerciseFormViewModel = new CreateExerciseFormViewModel();
                    createExerciseFormViewModel.Exercise = exercise;
                    createExerciseFormViewModel.Muscles = await _muscleService.FindAllAsync();
                    return View(createExerciseFormViewModel);
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: ExercisesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Exercise exercise)
        {
            try
            {
                var muscleIdAndName = Request.Form["createOrSelectedMuscle"];
                if (!string.IsNullOrEmpty(exercise.Name) && !string.IsNullOrEmpty(muscleIdAndName) && exercise.Id != 0)
                {
                    var exerciseInDb = await _exerciseService.FindByIdAsync(exercise.Id);
                    string[] splittedData = muscleIdAndName.ToString().Split("-");
                    int muscleId;
                    bool isSuccess = int.TryParse(splittedData[0], out muscleId);
                    if (exerciseInDb.Name == exercise.Name)
                    {
                        if (hasSameMuscle(exerciseInDb, muscleId))
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    //Nome alterado
                    exerciseInDb.Name = exercise.Name;
                    if (!hasSameMuscle(exerciseInDb, muscleId))
                    {
                        if (!isSuccess)
                        {
                            return NoContent();
                        }
                        exerciseInDb.Muscles.Clear();
                        var muscle = await _muscleService.FindByIdAsync(muscleId);
                        exerciseInDb.Muscles.Add(muscle);
                    }
                    await _exerciseService.UpdateAsync(exerciseInDb);
                    return RedirectToAction(nameof(Index));
                }
                return NoContent();
            }
            catch(DbConcurrencyException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "" });
            }
        }

        // GET: ExercisesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {

            if (id != 0)
            {
                var exercise = await _exerciseService.FindByIdAsync(id);
                if (exercise != null)
                {
                    return View(exercise);
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: ExercisesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeletePost(int id)
        {
            try
            {
                var exercise = await _exerciseService.FindByIdAsync(id);

                if (_exerciseService.ErrorReferentialIntegrity(exercise))
                {
                    throw new Referential_IntegrityException("You cannot remove this exercise, it is registered in a Workout.");
                }

                await _muscleService.RemoveMuscleByExerciseIdAsync(id);
                await _exerciseService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));


            }
            catch (Referential_IntegrityException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "You will need to remove the workout(s) first" });
            }
        }
        public ActionResult Error(string message, string solution)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                Solution = solution,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
        protected bool hasSameMuscle(Exercise exerciseInDataBase, int MuscleIdByForms)
        {
            var sameMuscle = exerciseInDataBase.Muscles.Where(x => x.Id == MuscleIdByForms);
            if (sameMuscle.Any())
            {
                return true;
            }
            return false;
        }
    }
}
