using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data
{
    public class WorkoutTrackerDbContext : DbContext
    {
        public DbSet<Workout>? Workout { get; set; }
        public DbSet<Exercise>? Exercise { get; set; }
        public DbSet<Category>? Category { get; set; }
        public DbSet<SetsAndReps>? SetsAndReps { get; set; }
        public DbSet<Muscle>? Muscle { get; set; }
        public WorkoutTrackerDbContext(DbContextOptions<WorkoutTrackerDbContext> options) : base(options)
        {
        }
    }
}
