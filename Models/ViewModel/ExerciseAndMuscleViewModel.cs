namespace WorkoutTracker.Models.ViewModel
{
    public class ExerciseAndMuscleViewModel
    {
        public ICollection<Exercise>? Exercises { get; set; }
        public ICollection<Muscle>? Muscles { get; set; }
        public Workout? Workout { get; set; }
        public ExerciseAndMuscleViewModel()
        {

        }
    }
}
