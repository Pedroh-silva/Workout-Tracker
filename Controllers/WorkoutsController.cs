using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Models;
using WorkoutTracker.Models.ViewModel;
using WorkoutTracker.Services;


namespace WorkoutTracker.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly WorkoutService _workoutService;
        private readonly ExerciseService _exerciseService;
        private readonly CategoryService _categoryService;
        public WorkoutsController(WorkoutService workoutService,ExerciseService exerciseService, CategoryService categoryService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
            _categoryService = categoryService;

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
            var listExercises = await _exerciseService.FindAllAsync();
            var listCategories = await _categoryService.FindAllAsync();
            CreateFormViewModel viewModel = new CreateFormViewModel { Categories = listCategories,Exercises = listExercises};
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
                
                if (!string.IsNullOrEmpty(selectedExercisesId))
                {
                    HttpContext.Session.SetString("WorkoutExercisesId", selectedExercisesId);
                    var selectedCategoryId = Request.Form["selectedCategory"];
                    if (!string.IsNullOrEmpty(selectedCategoryId))
                    {
                            HttpContext.Session.SetString("WorkoutCategoryId", selectedCategoryId);
                    }
                    return RedirectToAction(nameof(SetsAndReps),workout);
                }
                return NoContent();
            }
            catch
            {
                return View();
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
            CreateFormViewModel createFormViewModel = new CreateFormViewModel();

            createFormViewModel.Workout = workoutData;
            
            var exercisesId = HttpContext.Session.GetString("WorkoutExercisesId");
            if (exercisesId == null)
            {
                return RedirectToAction(nameof(Create));
            }
            string[] exercisesIdSplitted = exercisesId!.Split(",").ToArray();
            var allExercises = await _exerciseService.FindAllAsync();
            var listExercises = allExercises.Where(x => exercisesIdSplitted.Select(int.Parse).Contains(x.Id)).ToList();
           
            createFormViewModel.Exercises = listExercises;

            HttpContext.Session.Remove("WorkoutExercisesId");
            return View(createFormViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetsAndRepsPost(CreateFormViewModel createFormViewModel)
        {
            /*
             Form["SetsReps"] -> é recebido "IdDoExercício-QuantidadeDeSéries-QuantidadeDeRepetições"
             Todos separados por "-" caso seja adicionada mais séries e repetições
             por padrão são enviados separados por vírgula, ocorre a separação dos valores e o treino 
             adicionado é atualizado
            */

            Workout workoutData = new Workout();
            workoutData = createFormViewModel.Workout!;
            
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
                if(quantityOfExercises > initialSplit.Length)
                {
                    return NoContent();
                }
                //Adicionando categoria
                var categoriesId = HttpContext.Session.GetString("WorkoutCategoryId");
                string[] categoryIdSplitted = categoriesId!.Split(",").ToArray();
                var allCategories = await _categoryService.FindAllAsync();
                var categorySelected = allCategories.Where(x => categoryIdSplitted.Select(int.Parse).Contains(x.Id)).FirstOrDefault();
                if (categorySelected != null)
                {
                    workoutData.Categories.Add(categorySelected);
                }
                await _workoutService.InsertAsync(workoutData);

                HttpContext.Session.Remove("WorkoutCategoryId");
                var workoutjustAdded = await _workoutService.FindLastinDb();

                ICollection<SetsAndReps> setsAndReps = new List<SetsAndReps>();
                
                for (int i = 0; i < initialSplit.Length; i++)
                {
                    string[] finalSplit = initialSplit[i].ToString().Split("-");
                    var exerciseId = int.Parse(finalSplit[0]);
                    var quantity = int.Parse(finalSplit[1]);
                    var repetitions = int.Parse(finalSplit[2]);

                    SetsAndReps obj = new SetsAndReps() { Quantity = quantity, Repetitions = repetitions, WorkoutId = workoutjustAdded.Id, ExerciseId = exerciseId };
                    setsAndReps.Add(obj);
                }
               
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
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = await _workoutService.FindByIdAsync((int)id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        // POST: WorkoutsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
               await _workoutService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        //TODO: Add controllers for statistic and AddByCategory

    }
}
