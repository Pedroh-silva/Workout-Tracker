namespace WorkoutTracker.Models
{
    public class SetsAndReps
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public int Repeticoes { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }

        public Serie()
        {
        }
    }
}