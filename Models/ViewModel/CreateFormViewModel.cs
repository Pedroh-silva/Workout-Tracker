namespace WorkoutTracker.Models.ViewModel
{
    public class CreateFormViewModel
    {
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Exercise>? Exercises { get; set; }
        public Workout? Workout { get; set; }
        public CreateFormViewModel()
        {

        }
    }
}
