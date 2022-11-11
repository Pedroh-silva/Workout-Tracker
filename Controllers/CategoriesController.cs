using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Models;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Exceptions;

namespace WorkoutTracker.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryService _categoryService;
        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: CategoriesController
        public async Task<ActionResult> Index()
        {
            var allCategories = await _categoryService.FindAllAsync();
            return View(allCategories);
        }

        // GET: CategoriesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(Category category)
        {
            try
            {
                var categoryColor = Request.Form["categoryColor"];
                if (!string.IsNullOrEmpty(category.Name) && !string.IsNullOrEmpty(categoryColor))
                {
                    category.Color = categoryColor.ToString();
                    await _categoryService.Insert(category);
                    return RedirectToAction(nameof(Index));
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "" });
            }
        }

        // GET: CategoriesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CategoriesController/Edit/5
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

        // GET: CategoriesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id != 0)
            {
                var category = await _categoryService.FindByIdAsync(id);
                if (category != null)
                {
                    return View(category);
                }
                return RedirectToAction(nameof(Index));
            }
            return NoContent();
        }

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeletePost(int id)
        {
            try
            {
                if (id != 0)
                {
                    var category = await _categoryService.FindByIdAsync(id);
                    if (category == null) throw new NotFoundException("Id not found!");
                    await _categoryService.RemoveAsync(id);
                    return RedirectToAction(nameof(Index));
                }
                throw new NotFoundException("Id not found!");
            }
            catch (NotFoundException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "" });
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
    }
}
