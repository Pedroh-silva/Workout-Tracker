using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services.Exceptions;

namespace WorkoutTracker.Services
{
    public class MuscleService
    {
        private readonly WorkoutTrackerDbContext _context;
        public MuscleService(WorkoutTrackerDbContext context)
        {
            _context = context;
        }
        public async Task<List<Muscle>> FindAllAsync()
        {
            return await _context.Muscle!.Include(x => x.Exercises).OrderByDescending(x => x.Id).ToListAsync();
        }
        public async Task<Muscle> FindByIdAsync(int id)
        {
            var exercise = await _context.Muscle!.FirstOrDefaultAsync(x => x.Id == id);
            return exercise!;
        }
        public async Task InsertAsync(Muscle muscle)
        {
            _context.Add(muscle);
            await _context.SaveChangesAsync();

        }
        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Muscle!.FindAsync(id);
            _context.Muscle.Remove(obj!);
            await _context.SaveChangesAsync();

        }
        public async Task UpdateAsync(Muscle obj)
        {

            bool hasAny = await _context.Muscle!.AnyAsync(x => x.Id == obj.Id);
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
        public async Task RemoveMuscleByExerciseIdAsync(int exerciseId)
        {
            var exercise = await _context.Exercise!.FirstOrDefaultAsync(x => x.Id == exerciseId);
            var obj = await _context.Muscle!.Where(x => x.Exercises.Contains(exercise!)).Include(x => x.Exercises).FirstOrDefaultAsync();
            if(obj!.Exercises.Count > 1)
            {
                return;
            }
            _context.Muscle!.Remove(obj);
            await _context.SaveChangesAsync();


        }
    }
}
