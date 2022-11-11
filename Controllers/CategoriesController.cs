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

        // GET: CategoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category)
        {
            try
            {
                if (!string.IsNullOrEmpty(category.Name) && !string.IsNullOrEmpty(category.Color))
                {
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
        public async Task<ActionResult> Edit(int id)
        {
            if(id != 0)
            {
                var category = await _categoryService.FindByIdAsync(id);
                if (category == null) return NoContent();
                return View(category);
            }
            return NoContent();
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category category)
        {
            try
            {
                var categoryToBeEdited = await _categoryService.FindByIdAsync(category.Id);
                if (categoryToBeEdited == null) throw new NotFoundException("Id not found!");
                categoryToBeEdited.Name = category.Name;
                categoryToBeEdited.Color = category.Color;
                await _categoryService.UpdateAsync(categoryToBeEdited);
                return RedirectToAction(nameof(Index));
            }
            catch(NotFoundException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message, solution = "" });
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
