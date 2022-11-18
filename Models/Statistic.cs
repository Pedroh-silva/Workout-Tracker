namespace WorkoutTracker.Models
{
    public class Statistic
    {
        public int QuantityOfExercises { get; set; }
        public TimeSpan AverageDuration { get; set; }
        public int QuantityOfWorkouts { get; set; }
        public double AverageExercisesPerWorkout { get; set; }
        public TimeSpan AllDuration { get; set; }
        public List<string> MonthsWithAtLeastOneWorkout { get; set; } = new List<string>();
        //Monthly ----------------------------------------
        public int QuantityOfExercisesMonthly { get; set; }
        public int QuantityOfWorkoutsMonthly { get; set; }
        public double AverageExercisesPerWorkoutMonthly { get; set; }
        public TimeSpan AverageDurationMonthly { get; set; }
        public TimeSpan AllDurationMonthly { get; set; }
        public Statistic()
        {

        }
    }
}
