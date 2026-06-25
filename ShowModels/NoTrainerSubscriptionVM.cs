using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.ViewModels;

namespace Gym_kursovaya.ShowModels;

public partial class NoTrainerSubscriptionVM: ViewModelBase
{
    [ObservableProperty] private Subscription _selectedSubscription;
    [ObservableProperty] private Schedule _selectedSchedule;
    [ObservableProperty] private Client _client;
    [ObservableProperty] private int _trainingsLeft;
    [ObservableProperty] private int _daysLeft; 
    private MainWindowViewModel _mainWindowViewModel;
    private ExerciseRepository _exerciseRep;

    private ScheduleRepository _scheduleRep;
    /*
    [ObservableProperty] private string _clientName;
    [ObservableProperty] private char _clientGender;
    [ObservableProperty] private string _subscriptionTitle;
    */
    public NoTrainerSubscriptionVM(Subscription subscription, ScheduleRepository scheduleRepository, ClientRepository clientRepository, ExerciseRepository dayRepository)
    {
        SelectedSubscription = subscription;
        _scheduleRep = scheduleRepository;
        SelectedSchedule = scheduleRepository.GetById(subscription.ScheduleId);
        Client =  clientRepository.GetById(subscription.ClientId);
        DaysLeft = GetDaysLeft();
        TrainingsLeft = GetTrainingsLeft();
    }

    public int GetDaysLeft()
    {
        int num = 0;
        List<Exercise> days = _exerciseRep.GetBySubscriptionId(SelectedSubscription.Id);
        for (int i = days.Count - 1; i >= 0 && _mainWindowViewModel.TodayDate <= days[i].Date; i--)
        {
            num++;
        }
        return num;
    }

    public int GetTrainingsLeft()
    {
        int num = 0;
        List<Exercise> days = _exerciseRep.GetBySubscriptionId(SelectedSubscription.Id);
        foreach (Exercise day in days)
        {
            if (day.Date != _mainWindowViewModel.TodayDate && day.Status.Id == 1)
            {
                num++;
            }
        }

        return _scheduleRep.GetByClientId(Client.Id).DaysAmount - num;
    }
}