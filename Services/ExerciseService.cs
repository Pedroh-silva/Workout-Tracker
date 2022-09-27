using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services.Exceptions;

namespace WorkoutTracker.Services
{
    public class ExerciseService
    {
        private readonly WorkoutTrackerDbContext _context;
        public ExerciseService(WorkoutTrackerDbContext context)
        {
            _context = context;
        }
        public async Task<List<Exercise>> FindAllAsync()
        {
            return await _context.Exercise!.Include(x => x.Muscles).OrderByDescending(x => x.Name).ToListAsync();
        }
        public async Task<Exercise> FindByIdAsync(int id)
        {
            var exercise = await _context.Exercise!.FirstOrDefaultAsync(x => x.Id == id);
            return exercise!;
        }
        public async Task Insert(Exercise exercise)
        {
            _context.Add(exercise);
            await _context.SaveChangesAsync();

        }
        public async Task Remove(int id)
        {
            var obj = await _context.Exercise!.FindAsync(id);
            _context.Exercise.Remove(obj!);
            await _context.SaveChangesAsync();

        }
        public async Task UpdateAsync(Exercise obj)
        {

            bool hasAny = await _context.Exercise!.AnyAsync(x => x.Id == obj.Id);
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
