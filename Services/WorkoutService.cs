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
            return await _context.Workout!.Include(x => x.Exercises).ThenInclude(x => x.SetsAndReps).Include(x => x.Categories).OrderByDescending(x => x.DateTime).ToListAsync();
        }
        public async Task<Workout> FindByIdAsync(int id)
        {
            var workout = await _context.Workout!.Include(x=>x.Exercises).ThenInclude(x => x.SetsAndReps).Include(x=>x.Categories).FirstOrDefaultAsync(x => x.Id == id);
            return workout!;
        }
        public async Task<Workout> FindLastinDbAsync()
        {
            var workout = await _context.Workout!.Include(x=>x.Exercises).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return workout!;
        }
        public async Task InsertAsync(Workout workout)
        {
            
            _context.Add(workout);
            await _context.SaveChangesAsync();
            

        }
        public async Task RemoveAsync(int workoutId)
        {
            var obj = await _context.Workout!.FindAsync(workoutId);
            var setsAndReps = await _context.SetsAndReps!.Where(x => x.WorkoutId == workoutId).ToListAsync();
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
