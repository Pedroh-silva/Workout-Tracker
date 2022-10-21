using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<WorkoutTrackerDbContext>(options => options.UseSqlServer("Server=localhost;Database=db_Workout_Tracker;Trusted_Connection=True;"));
builder.Services.AddScoped<WorkoutService>();
builder.Services.AddScoped<ExerciseService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<SetsAndRepsService>();
builder.Services.AddScoped<MuscleService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Workouts}/{action=Index}/{id?}");

app.Run();
