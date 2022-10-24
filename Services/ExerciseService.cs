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
            return await _context.Exercise!.Include(x => x.Muscles).Include(x => x.SetsAndReps).OrderByDescending(x => x.Name).ToListAsync();
        }
        public async Task<Exercise> FindByIdAsync(int id)
        {
            var exercise = await _context.Exercise!.Include(x => x.Muscles).Include(x => x.Workouts).FirstOrDefaultAsync(x => x.Id == id);
            return exercise!;
        }
        public async Task InsertAsync(Exercise exercise)
        {
            _context.Add(exercise);
            await _context.SaveChangesAsync();

        }
        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Exercise!.FindAsync(id);
            try
            {
                _context.Exercise!.Remove(obj!);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new ApplicationException("Something went wrong when removing this exercise.");
            }
            
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
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
        public bool ErrorReferentialIntegrity(Exercise obj)
        {
            if (obj!.Workouts.Count() != 0)
            {
                return true;
            }
            return false;
        }
    }
}
