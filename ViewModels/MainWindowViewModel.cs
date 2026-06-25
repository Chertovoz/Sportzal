using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private DateTime _todayDate;
    [ObservableProperty] private MuscleVM _muscleViewModel;
    [ObservableProperty] private ClientVM _clientViewModel;
    [ObservableProperty] private TrainerVM _trainerViewModel;
    [ObservableProperty] private ProgramVM _programViewModel;
    [ObservableProperty] private SubscriptionVM _subscriptionViewModel;
    IServiceProvider _serviceProvider;
    public MainWindowViewModel(IServiceProvider serviceProvider,  ScheduleRepository scheduleRep,ProgrammRepository programmRep, MuscleVM muscleVm, TrainerVM trainerVm, ClientVM clientVM, ProgramVM programVM, SubscriptionVM subscriptionVM)
    {
        _serviceProvider = serviceProvider;
        _todayDate = DateTime.Today;
        MuscleViewModel = muscleVm;
        ClientViewModel = clientVM;
        TrainerViewModel = trainerVm;
        ProgramViewModel = programVM;
        SubscriptionViewModel = subscriptionVM;
    }

    [RelayCommand]
    public void MarkClients()
    {
        var vm = ActivatorUtilities.CreateInstance<MarkClientsVM>(_serviceProvider);
        var win = _serviceProvider.GetRequiredService<MarkWindow>();
        win.DataContext = vm;
        win.Show();
    }
    
    
}