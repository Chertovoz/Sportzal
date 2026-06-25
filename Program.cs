using Avalonia;
using System;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.ShowModels;
using Gym_kursovaya.ViewModels;
using Gym_kursovaya.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gym_kursovaya;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder().
            ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("databaseSettings.json")
                    .AddEnvironmentVariables();
            }).
            ConfigureServices((c,s) =>
            {
                s.Configure<DatabaseConnection>(c.Configuration.
                    GetSection("DatabaseConnection"));
                s.AddTransient<MainWindowViewModel>();
                s.AddTransient<MainWindow>();
                s.AddTransient<ShowSubscription>();
                //repos
                s.AddTransient<ClientRepository>();
                s.AddTransient<ExerciseRepository>();
                s.AddTransient<ExerciseStatusRepository>();
                s.AddTransient<ExerciseProgramRepository>();
                s.AddTransient<MuscleRepository>();
                s.AddTransient<MuscleProgramRepository>();
                s.AddTransient<ProgrammRepository>();
                s.AddTransient<ScheduleRepository>();
                s.AddTransient<SubscriptionRepository>();
                s.AddTransient<TrainerRepository>();
                
                //repos end
                //editvm 
                s.AddTransient<ClientEditVM>();
                s.AddTransient<MuscleEditVM>();
                s.AddTransient<ScheduleEditVM>();
                s.AddTransient<SybscriptionEditVM>();
                s.AddTransient<TrainerEditVM>();
                s.AddTransient<ProgrammEditVM>();
                //editvm end
                //editwindow 
                s.AddTransient<EditClientWindow>();
                s.AddTransient<EditMuscleWindow>();
                s.AddTransient<EditProgrammWindow>();
                s.AddTransient<EditSubscriptionWIndow>();
                s.AddTransient<EditTrainerWindow>();
                s.AddTransient<NoTrainerAbonementWindow>();
                s.AddTransient<MarkWindow>();
                //editwindow end
                
                //vm
                s.AddTransient<SubscriptionVM>();
                s.AddTransient<ClientVM>();
                s.AddTransient<MuscleVM>();
                s.AddTransient<ProgramVM>();
                s.AddTransient<TrainerVM>();
                s.AddTransient<MarkClientsVM>();
                s.AddTransient<Trainer>();
                s.AddTransient<ProgramVM>();
            }).
            Build();
        BuildAvaloniaApp(host.Services)
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp(IServiceProvider provider)
        => AppBuilder.Configure(()=>new App(provider))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}