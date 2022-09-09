using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services.Exceptions;

namespace WorkoutTracker.Services
{
    public class WorkoutService
    {
        private readonly WorkoutTrackerDbContext _context;
        public WorkoutService(WorkoutTrackerDbContext context)
        {
            _context = context;
        }
        public async Task<List<Workout>> FindAllAsync()
        {
            return await _context.Workout!.OrderByDescending(x => x.DateTime).ToListAsync();
        }
        public async Task<Workout> FindByIdAsync(int id)
        {
            var workout = await _context.Workout!.FirstOrDefaultAsync(x => x.Id == id);
            return workout!;
        }
        public async Task Insert(Workout workout)
        {
            _context.Add(workout);
            await _context.SaveChangesAsync();

        }
        public async Task Remove(int id)
        {
            var obj = await _context.Workout!.FindAsync(id);
            var setsAndReps = await _context.SetsAndReps!.Where(x => x.WorkoutId == id).ToListAsync();
            _context.SetsAndReps!.RemoveRange(setsAndReps);
            _context.Workout.Remove(obj!);
            await _context.SaveChangesAsync();

        }
        public async Task UpdateAsync(Workout obj)
        {

            bool hasAny = await _context.Workout!.AnyAsync(x => x.Id == obj.Id);
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
