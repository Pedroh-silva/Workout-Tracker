using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services.Exceptions;

namespace WorkoutTracker.Services
{
    public class CategoryService
    {
        private readonly WorkoutTrackerDbContext _context;
        public CategoryService(WorkoutTrackerDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> FindAllAsync()
        {
            return await _context.Category!.OrderByDescending(x => x.Name).Include(x => x.Workouts).ThenInclude(x => x.Exercises).ToListAsync();
        }
        public async Task<Category> FindByIdAsync(int id)
        {
            var category = await _context.Category!.FirstOrDefaultAsync(x => x.Id == id);
            return category!;
        }
        public async Task Insert(Category category)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();

        }
        public async Task Remove(int id)
        {
            var obj = await _context.Category!.FindAsync(id);
            _context.Category.Remove(obj!);
            await _context.SaveChangesAsync();

        }
        public async Task UpdateAsync(Category obj)
        {

            bool hasAny = await _context.Category!.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }
            try
            {
                _context.Update(obj);
                _context.SaveChanges();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
