namespace WorkoutTracker.Models.ViewModel
{
    public class CreateExerciseFormViewModel
    {
        public Exercise? Exercise { get; set; }
        public ICollection<Muscle>? Muscles { get; set; }
        public CreateExerciseFormViewModel()
        {

        }
    }
}
