namespace WorkoutTracker.Models
{
    public class Muscle
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();

        public Muscle()
        {

        }
    }
}