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
        private readonly SetsAndRepsService _setsAndRepsService;
        public WorkoutsController(WorkoutService workoutService, ExerciseService exerciseService, CategoryService categoryService, SetsAndRepsService setsAndRepsService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
            _categoryService = categoryService;
            _setsAndRepsService = setsAndRepsService;

        }
        //TODO: CRIAR PÁGINA PARA ADICIONAR NOVAS CATEGORIAS
        //TODO: CRIAR PÁGINAS DE ESTATÍSTICA
        //TODO: CRIAR FUNÇÕES DA PÁGINA ESTATÍSTICA
        //TODO: ADICIONAR EDIÇÃO E DELEÇÃO DE EXERCÍCIOS
        //TODO: VERIFICAR PORQUE ADICONAR PELA CATEGORIA NÃO ESTÁ FUNCIONANDO
        

        // GET: WorkoutsController
        public async Task<ActionResult> Index()
        {
            var workouts = await _workoutService.FindAllAsync();
            return View(workouts);
        }

        // GET: WorkoutsController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id != null)
            {
                var workout = await _workoutService.FindByIdAsync((int)id);
                if (workout == null)
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
            CreateFormViewModel viewModel = new CreateFormViewModel { Categories = listCategories, Exercises = listExercises };
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
            var listExercises = await _exerciseService.FindAllAsync();
            var listCategories = await _categoryService.FindAllAsync();
            var workout = await _workoutService.FindByIdAsync(id);
            CreateFormViewModel viewModel = new CreateFormViewModel { Categories = listCategories, Exercises = listExercises, Workout = workout };
            return View(viewModel);
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

            var listExIdSetsReps = Request.Form["SetsReps"];
            Workout workoutToBeEdited = await _workoutService.FindByIdAsync(workout.Id);

            if (workoutToBeEdited == null)
            {
                return BadRequest();
            }

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

                workoutToBeEdited.Exercises.Clear();
                workoutToBeEdited.Categories.Clear();
                workoutToBeEdited.Name = workout.Name;
                workoutToBeEdited.DateTime = workout.DateTime;
                workoutToBeEdited.Duration = workout.Duration;
                //Adicionando categoria
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

            return NoContent();
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
            catch
            {
                return View();
            }
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
