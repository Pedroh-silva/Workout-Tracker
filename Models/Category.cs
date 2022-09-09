using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Por favor, coloque um nome para a categoria")]
        [Display(Name = "Nome")]
        public string? Name { get; set; }
        [Display(Name = "Cor")]
        public string? Color { get; set; }
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

        public Category()
        {

        }
    }
}