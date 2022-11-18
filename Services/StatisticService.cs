using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    public class StatisticService
    {
        public int QuantityOfExercises(List<Workout> workouts)
        {
            var exercisesDone = 0;
            foreach (var obj in workouts)
            {

                exercisesDone += obj.Exercises.Count;
            }
            return exercisesDone;
        }
        public TimeSpan AverageDuration(List<Workout> workouts)
        {
            TimeSpan averageDuration = new TimeSpan();
            TimeSpan totalduration = new TimeSpan();

            foreach (var obj in workouts)
            {

                totalduration += obj.Duration;
            }
            var quantityOfWorkouts = QuantityOfWorkouts(workouts);
            averageDuration = totalduration / quantityOfWorkouts;

            return averageDuration;
        }
        public TimeSpan AllDuration(List<Workout> workouts)
        {
            TimeSpan duration = new TimeSpan();
            foreach (var obj in workouts)
            {
                duration += obj.Duration;
            }
            return duration;
        }
        public int QuantityOfWorkouts(List<Workout> workouts)
        {
            var quantityOfWorkouts = workouts.Count;

            return quantityOfWorkouts;
        }
        public double AverageExercisesPerWorkout(List<Workout> workouts)
        {
            double quantityOfWorkouts = QuantityOfWorkouts(workouts);
            double quantityOfExercises = QuantityOfExercises(workouts);
            double average = quantityOfExercises / quantityOfWorkouts;
            return average;
        }
        public List<string> MuscleDistribution(List<Workout> workouts)
        {
            List<string> muscleDistribution = new List<string>();

            if (workouts.Count == 0) return muscleDistribution;

            foreach (var obj in workouts)
            {
                foreach (var exercise in obj.Exercises)
                {
                    foreach (var muscles in exercise.Muscles)
                    {
                        muscleDistribution.Add(muscles.Name!);
                    }
                }
            }
            return muscleDistribution;
        }
        public List<string> MonthsWithAtLeastOneWorkout(List<Workout> workouts)
        {
            List<string> monthsWithWorkoutsDuplicate = new List<string>();
            foreach(var obj in workouts)
            {
                monthsWithWorkoutsDuplicate.Add(obj.DateTime.ToString("MMMM"));
            }
            List<string> monthsWithWorkoutsWithoutDuplicate = new List<string>();
            monthsWithWorkoutsWithoutDuplicate.AddRange(monthsWithWorkoutsDuplicate.Distinct().ToList());
            return monthsWithWorkoutsWithoutDuplicate; 
        }
        //MONTHLY ----------------------------------------------------------------------
        public int QuantityOfExercisesMonthly(List<Workout> workouts, string monthName)
        {
            var exercisesDone = 0;
            foreach (var obj in workouts)
            {
                if(obj.DateTime.ToString("MMMM") == monthName)
                {
                    exercisesDone += obj.Exercises.Count;
                }
            }
            return exercisesDone;
        }
        public int QuantityOfWorkoutsMonthly(List<Workout> workouts, string monthName)
        {
            var quantityOfWorkouts = 0;
            foreach (var obj in workouts)
            {
                if (obj.DateTime.ToString("MMMM") == monthName)
                {
                    quantityOfWorkouts ++;
                }
            }
            return quantityOfWorkouts;
        }
        public double AverageExercisesPerWorkoutMonthly(List<Workout> workouts, string monthName)
        {
            double quantityOfWorkouts = QuantityOfWorkoutsMonthly(workouts, monthName);
            double quantityOfExercises = QuantityOfExercisesMonthly(workouts, monthName);
            double average = quantityOfExercises / quantityOfWorkouts;
            return average;
        }
        public TimeSpan AverageDurationMonthly(List<Workout> workouts, string monthName)
        {
            TimeSpan averageDuration = new TimeSpan();
            TimeSpan totalduration = new TimeSpan();

            foreach (var obj in workouts)
            {
                if(obj.DateTime.ToString("MMMM") == monthName)
                {
                    totalduration += obj.Duration;
                }
            }
            var quantityOfWorkouts = QuantityOfWorkoutsMonthly(workouts,monthName);
            averageDuration = totalduration / quantityOfWorkouts;

            return averageDuration;
        }
        public TimeSpan AllDurationMonthly(List<Workout> workouts, string monthName)
        {
            TimeSpan duration = new TimeSpan();
            foreach (var obj in workouts)
            {
                if(obj.DateTime.ToString("MMMM") == monthName)
                {
                    duration += obj.Duration;
                }
            }
            return duration;
        }
        //---------------------------------------------------------------------------------------
        public string CreatePieGraph(string title, string data)
        {
            // O formato dos dados devem ser => ['Work',11],['Eat',2],['Commute',2],['Watch TV',2] //
            string graf = @"<script type='text/javascript' src='https://www.gstatic.com/charts/loader.js'></script>
                            <script type = 'text/javascript'>
                            google.charts.load('current', { packages: ['corechart']});
                            google.charts.setOnLoadCallback(drawChart);
                            function drawChart(){   
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                " + data + @"
                                ]);

                            var options = {
                             title:'" + title + @"',
                             pieHole: 0.5,
                             backgroundColor: { fill:'transparent' },
                             legend:{position: 'bottom', textStyle: {color: 'white'}},
                             chartArea: {'width': '100%', 'height': '80%'},
                             colors:['#003d0a','#018d17','#01b21d','#4f8b35','#69b847','#8cf45f','#4d703e','#7cb265','#a5ef86'],
                            };

                            var chart = new google.visualization.PieChart(document.getElementById
                             ('donutchart_" + title.Replace(" ", "") + @"'));
                             chart.draw(data, options);
                             
                            }
                            </script>
                            <div id='donutchart_" + title.Replace(" ", "") + @"' style='min-height: 500px; '></div>";
            return graf;
        }
    }
}
