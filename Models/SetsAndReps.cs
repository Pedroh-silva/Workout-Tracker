namespace WorkoutTracker.Models
{
    public class SetsAndReps
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Repetitions { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }

        public SetsAndReps()
        {

        }
    }
}