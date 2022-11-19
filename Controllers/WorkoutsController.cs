using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Models;
using WorkoutTracker.Models.ViewModel;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Exceptions;

namespace WorkoutTracker.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly WorkoutService _workoutService;
        private readonly ExerciseService _exerciseService;
        private readonly CategoryService _categoryService;
        private readonly SetsAndRepsService _setsAndRepsService;
        private readonly StatisticService _statisticService;
        private readonly MuscleService _muscleService;
        public WorkoutsController(WorkoutService workoutService, ExerciseService exerciseService, CategoryService categoryService, SetsAndRepsService setsAndRepsService, StatisticService statisticService, MuscleService muscleService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
            _categoryService = categoryService;
            _setsAndRepsService = setsAndRepsService;
            _statisticService = statisticService;
            _muscleService = muscleService;

        }

        // GET: WorkoutsController
        public async Task<ActionResult> Index()
        {
            var workouts = await _workoutService.FindAllAsync();
            return View(workouts);
        }

        // GET: WorkoutsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id != 0)
            {
                var workout = await _workoutService.FindByIdAsync(id);
                if (workout != null)
                {
                    return View(workout);

                }
                return NotFound();
            }
            return BadRequest();
        }

        // GET: WorkoutsController/Create
        public async Task<ActionResult> Create()
        {
            var listExercises = await _exerciseService.FindAllAsync();
            var listCategories = await _categoryService.FindAllAsync();
            var listMuscles = await _muscleService.FindAllAsync();
            CreateFormViewModel viewModel = new CreateFormViewModel { Categories = listCategories, Exercises = listExercises, Muscles = listMuscles };
            return View(viewModel);
        }

        // POST: WorkoutsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Workout workout)
        {
            try
            {

                var selectedExercisesId = Request.Form["selectedExercise"];
                if (isExerciseFormNull(selectedExercisesId))
                {
                    return NoContent();
                }
                HttpContext.Session.SetString("WorkoutExercisesId", selectedExercisesId);
                var selectedCategoryId = Request.Form["selectedCategory"];
                if (!isCategoryFormNull(selectedCategoryId))
                {
                    HttpContext.Session.SetString("WorkoutCategoryId", selectedCategoryId);
                }
                return RedirectToAction(nameof(SetsAndReps), workout);


            }
            catch
            {
                return NoContent();
            }
        }
        public async Task<ActionResult> SetsAndReps(Workout workoutData)
        {
            /* workoutData -> Essa variável possui apenas dados como nome, data e duração
             será adicionado os exercícios e a categoria posteriormente após o envio do forms
             com as séries e repetições */

            if (workoutData == null)
            {
                return RedirectToAction(nameof(Create));
            }
            var exercisesId = HttpContext.Session.GetString("WorkoutExercisesId");
            if (exercisesId == null)
            {
                return RedirectToAction(nameof(Create));
            }
            CreateFormViewModel createFormViewModel = new CreateFormViewModel();

            createFormViewModel.Workout = workoutData;

            var listExercises = await CreateListOfExerciseBySession(exercisesId);
            createFormViewModel.Exercises = listExercises;

            HttpContext.Session.Remove("WorkoutExercisesId");
            return View(createFormViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetsAndRepsPost(Workout workout)
        {
            /*
             Form["SetsReps"] -> é recebido "IdDoExercício-QuantidadeDeSéries-QuantidadeDeRepetições"
             Todos separados por "-" caso seja adicionada mais séries e repetições
             por padrão são enviados separados por vírgula, ocorre a separação dos valores e o treino 
             adicionado é atualizado
            */

            var listExIdSetsReps = Request.Form["SetsReps"];
            var formExerciseCount = Request.Form["quantityOfExercises"].ToString();
            int quantityOfExercises;
            bool success = int.TryParse(formExerciseCount, out quantityOfExercises);
            if (!success)
            {
                return NoContent();
            }
            if (!string.IsNullOrEmpty(listExIdSetsReps))
            {
                string[] initialSplit = listExIdSetsReps.ToString().Split(",");
                if (quantityOfExercises > initialSplit.Length)
                {
                    return NoContent();
                }

                var categoriesId = HttpContext.Session.GetString("WorkoutCategoryId");
                if (categoriesId != null)
                {
                    bool isEditing = false;
                    var categorySelected = await AddCategoryToWorkout(isEditing, categoriesId);
                    if (categorySelected != null)
                    {
                        workout.Categories.Add(categorySelected);

                    }
                }
                await _workoutService.InsertAsync(workout);
                var workoutjustAdded = await _workoutService.FindLastinDbAsync();

                var setsAndReps = CreateListOfSetsAndReps(initialSplit, workoutjustAdded.Id);

                foreach (var obj in setsAndReps)
                {
                    var exercise = await _exerciseService.FindByIdAsync(obj.ExerciseId);
                    exercise.SetsAndReps.Add(obj);
                    workoutjustAdded.Exercises.Add(exercise);
                }

                await _workoutService.UpdateAsync(workoutjustAdded);
                return RedirectToAction(nameof(Index));

            }

            return NoContent();
        }

        // GET: WorkoutsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id != 0)
            {
                var workout = await _workoutService.FindByIdAsync(id);
                if (workout != null)
                {
                    var listExercises = await _exerciseService.FindAllAsync();
                    var listCategories = await _categoryService.FindAllAsync();
                    CreateFormViewModel viewModel = new CreateFormViewModel { Categories = listCategories, Exercises = listExercises, Workout = workout };
                    return View(viewModel);
                }
                return NoContent();
            }
            return NoContent();
        }

        // POST: WorkoutsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Workout workout)
        {
            try
            {

                var selectedExercisesId = Request.Form["selectedExercise"];
                if (isExerciseFormNull(selectedExercisesId))
                {
                    return NoContent();
                }
                HttpContext.Session.SetString("WorkoutExercisesIdEdit", selectedExercisesId);
                var selectedCategoryId = Request.Form["selectedCategory"];
                if (!isCategoryFormNull(selectedCategoryId))
                {
                    HttpContext.Session.SetString("WorkoutCategoryIdEdit", selectedCategoryId);
                }
                return RedirectToAction(nameof(EditSetsAndReps), workout);

            }
            catch
            {
                return NoContent();
            }
        }
        public async Task<ActionResult> EditSetsAndReps(Workout workout)
        {
            if (workout == null)
            {
                return NoContent();
            }

            var exercisesId = HttpContext.Session.GetString("WorkoutExercisesIdEdit");
            if (exercisesId == null)
            {
                return NoContent();
            }

            var listExercises = await CreateListOfExerciseBySession(exercisesId);
            workout.Exercises = listExercises;

            HttpContext.Session.Remove("WorkoutExercisesIdEdit");
            return View(workout);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSetsAndRepsPost(Workout workout)
        {
            try
            {
                var listExIdSetsReps = Request.Form["SetsReps"];
                Workout workoutToBeEdited = await _workoutService.FindByIdAsync(workout.Id);
                if (workoutToBeEdited == null) throw new NotFoundException("Id not found");

                var formExerciseCount = Request.Form["quantityOfExercises"].ToString();
                int quantityOfExercises;
                bool success = int.TryParse(formExerciseCount, out quantityOfExercises);
                if (!success)
                {
                    return NoContent();
                }

                if (string.IsNullOrEmpty(listExIdSetsReps))
                {
                    return NoContent();
                }
                string[] initialSplit = listExIdSetsReps.ToString().Split(",");
                if (quantityOfExercises > initialSplit.Length)
                {
                    return NoContent();
                }

                workoutToBeEdited.Exercises.Clear();
                workoutToBeEdited.Categories.Clear();
                workoutToBeEdited.Name = workout.Name;
                workoutToBeEdited.DateTime = workout.DateTime;
                workoutToBeEdited.Duration = workout.Duration;
                //Adicionando categoria (não é necessário que um treino tenha realmente uma categoria)
                var categoriesId = HttpContext.Session.GetString("WorkoutCategoryIdEdit");
                if (categoriesId != null)
                {
                    bool isEditing = true;
                    var categorySelected = await AddCategoryToWorkout(isEditing, categoriesId);
                    if (categorySelected != null)
                    {
                        workoutToBeEdited.Categories.Add(categorySelected);

                    }
                }

                var setsAndReps = CreateListOfSetsAndReps(initialSplit, workoutToBeEdited.Id);

                await _setsAndRepsService.RemoveAllByWorkoutIdAsync(workoutToBeEdited.Id);

                foreach (var obj in setsAndReps)
                {
                    var exercise = await _exerciseService.FindByIdAsync(obj.ExerciseId);
                    exercise.SetsAndReps.Add(obj);
                    workoutToBeEdited.Exercises.Add(exercise);
                }
                await _workoutService.UpdateAsync(workoutToBeEdited);

                return RedirectToAction(nameof(Index));

            }
            catch (DbConcurrencyException)
            {
                return BadRequest();
            }
            catch (NotFoundException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "" });
            }

        }
        // GET: WorkoutsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id != 0)
            {
                var obj = await _workoutService.FindByIdAsync(id);
                if (obj != null)
                {
                    return View(obj);

                }
                return NoContent();
            }
            return NoContent();

        }

        // POST: WorkoutsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeletePost(int id)
        {
            try
            {
                var workout = await _workoutService.FindByIdAsync(id);
                if(workout == null) throw new NotFoundException("Id not found");
                await _workoutService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "" });
            }
        }
        public async Task<ActionResult> AddByCategory()
        {
            AddByCategoryFormViewModel addByCategoryFormViewModel = new AddByCategoryFormViewModel();
            addByCategoryFormViewModel.Categories = await _categoryService.FindAllAsync();
            return View(addByCategoryFormViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddByCategoryPost()
        {
            try
            {

                var exercisesId = Request.Form["selectedExercise"];
                var durationforms = Request.Form["Workout.Duration"].ToString();
                var dateTimeforms = Request.Form["workout.DateTime"].ToString();

                if (!string.IsNullOrEmpty(exercisesId) && !string.IsNullOrEmpty(dateTimeforms) && !string.IsNullOrEmpty(durationforms))
                {
                    var dateTime = DateTime.Parse(dateTimeforms);
                    var duration = TimeSpan.Parse(durationforms);
                    var id = int.Parse(Request.Form["WorkoutId"].ToString());
                    var workoutInDb = await _workoutService.FindByIdAsync(id);
                    if (workoutInDb == null) throw new NotFoundException("Id not found");
                    Workout workout = new Workout { Name = workoutInDb.Name, DateTime = dateTime, Duration = duration };

                    HttpContext.Session.SetString("WorkoutExercisesId", exercisesId);
                    var selectedCategoryId = Request.Form["selectedCategory"];
                    if (!string.IsNullOrEmpty(selectedCategoryId))
                    {
                        HttpContext.Session.SetString("WorkoutCategoryId", selectedCategoryId);
                    }

                    return RedirectToAction(nameof(SetsAndReps), workout);
                }
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "" });
            }
        }
        public async Task<ActionResult> Statistic()
        {
            var workouts = await _workoutService.FindAllAsync();
            Statistic statistic = new Statistic();
            if (workouts.Count != 0)
            {
                statistic.QuantityOfWorkouts = _statisticService.QuantityOfWorkouts(workouts);
                statistic.AverageDuration = _statisticService.AverageDuration(workouts);
                statistic.QuantityOfExercises = _statisticService.QuantityOfExercises(workouts);
                statistic.AverageExercisesPerWorkout = _statisticService.AverageExercisesPerWorkout(workouts);
                statistic.AllDuration = _statisticService.AllDuration(workouts);
                statistic.MonthsWithAtLeastOneWorkout = _statisticService.MonthsWithAtLeastOneWorkout(workouts);

                //Create pie graph from Google Charts API
                var muscleDistribution = _statisticService.MuscleDistribution(workouts);
                var quantityOfExercisesPerMuscleName = muscleDistribution.GroupBy(x => x).Select(y => new {y.Key, TotalExercises = y.Count() });
                string data = "";
                foreach(var obj in quantityOfExercisesPerMuscleName)
                {
                    data += "['" + obj.Key + "'," + obj.TotalExercises + "],";
                }
                data = data.Substring(0, data.Length - 1);
                ViewBag.CreatePieGraph = _statisticService.CreatePieGraph("",data);
                return View(statistic);
            }
            ViewBag.CreatePieGraph = "";
            return View(statistic);

        }
        public async Task<PartialViewResult> GetWorkoutByMonth(string id)
        {
            var workouts = await _workoutService.FindAllAsync();
            Statistic statistic = new Statistic();
            statistic.QuantityOfWorkoutsMonthly = _statisticService.QuantityOfWorkoutsMonthly(workouts,id);
            statistic.AverageDurationMonthly = _statisticService.AverageDurationMonthly(workouts, id);
            statistic.QuantityOfExercisesMonthly = _statisticService.QuantityOfExercisesMonthly(workouts, id);
            statistic.AverageExercisesPerWorkoutMonthly = _statisticService.AverageExercisesPerWorkoutMonthly(workouts, id);
            statistic.AllDurationMonthly = _statisticService.AllDurationMonthly(workouts, id);
            return PartialView(statistic);
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
        private bool isExerciseFormNull(string form)
        {
            if (string.IsNullOrEmpty(form))
            {
                return true;
            }
            return false;
        }
        private bool isCategoryFormNull(string form)
        {
            if (string.IsNullOrEmpty(form))
            {
                return true;
            }
            return false;
        }
        private async Task<Category> AddCategoryToWorkout(bool isEditing, string categoriesId)
        {

            string[] categoryIdSplitted = categoriesId!.Split(",").ToArray();
            var allCategories = await _categoryService.FindAllAsync();
            var categorySelected = allCategories.Where(x => categoryIdSplitted.Select(int.Parse).Contains(x.Id)).FirstOrDefault();
            if (isEditing)
            {
                HttpContext.Session.Remove("WorkoutCategoryIdEdit");
                return categorySelected!;
            }
            else
            {
                HttpContext.Session.Remove("WorkoutCategoryId");
                return categorySelected!;
            }
        }
        private ICollection<SetsAndReps> CreateListOfSetsAndReps(string[] initialSplit, int Workoutid)
        {
            ICollection<SetsAndReps> setsAndReps = new List<SetsAndReps>();

            for (int i = 0; i < initialSplit.Length; i++)
            {
                string[] finalSplit = initialSplit[i].ToString().Split("-");
                var exerciseId = int.Parse(finalSplit[0]);
                var quantity = int.Parse(finalSplit[1]);
                var repetitions = int.Parse(finalSplit[2]);

                SetsAndReps obj = new SetsAndReps() { Quantity = quantity, Repetitions = repetitions, WorkoutId = Workoutid, ExerciseId = exerciseId };
                setsAndReps.Add(obj);
            }
            return setsAndReps;
        }
        private async Task<List<Exercise>> CreateListOfExerciseBySession(string exercisesId)
        {
            string[] exercisesIdSplitted = exercisesId!.Split(",").ToArray();
            var allExercises = await _exerciseService.FindAllAsync();
            var listExercises = allExercises.Where(x => exercisesIdSplitted.Select(int.Parse).Contains(x.Id)).ToList();
            return listExercises;
        }
    }
}
