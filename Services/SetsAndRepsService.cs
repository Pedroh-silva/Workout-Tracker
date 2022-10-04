using WorkoutTracker.Data;

namespace WorkoutTracker.Services
{
    public class SetsAndRepsService
    {
        private readonly WorkoutTrackerDbContext _context;
        public SetsAndRepsService(WorkoutTrackerDbContext context)
        {
            _context = context;
        }
        public async Task RemoveAllByWorkoutIdAsync(int id)
        {
            var obj = _context.SetsAndReps!.Where(x => x.WorkoutId == id).ToList();
            _context.SetsAndReps!.RemoveRange(obj);
            await _context.SaveChangesAsync();

        }
    }
}
