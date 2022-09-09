using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class Workout
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Por favor, coloque o nome do treino")]
        [Display(Name = "Nome do treino")]
        public string? Name { get; set; } 
        [Required(ErrorMessage = "Por favor, coloque a data e hora do treino")]
        [Display(Name = "Data e hora do treino")]
        public DateTime DateTime { get; set; }
        [Required(ErrorMessage = "Por favor, coloque a duração do treino")]
        [DataType(DataType.Time)]
        [Display(Name = "Duração do treino")]
        public TimeSpan Duration { get; set; }
        [Display(Name = "Exercícios")]
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
        [Display(Name = "Categorias")]
        public ICollection<Category> Categories { get; set; } = new List<Category>();


        public Workout()
        {

        }
    }
}
