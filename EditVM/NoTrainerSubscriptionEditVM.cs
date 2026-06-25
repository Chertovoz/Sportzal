using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;

namespace Gym_kursovaya.ViewModels;

public partial class NoTrainerSubscriptionEditVM: ViewModelBase
{
    
    //класс для создания/редактирования абонемента, у которого нет тренера
    [ObservableProperty] private Client _selectedClient;
    [ObservableProperty] private List<Schedule> _schedules;
    [ObservableProperty] private Schedule _selectedSchedule;
    [ObservableProperty] private Subscription _clientSubscription;
    [ObservableProperty] private DateTime _clientStartDate;
    [ObservableProperty] private DateTime _clientEndDate;
    private SubscriptionRepository _subscriptionRep;
    private Action _closeAction;
    public NoTrainerSubscriptionEditVM( ScheduleRepository scheduleRep, SubscriptionRepository subscriptionRep )
    {
        SelectedSchedule = scheduleRep.GetByClientId(SelectedClient.Id);
        ClientSubscription = subscriptionRep.GetValidByClientId(SelectedClient.Id);
        ClientStartDate = subscriptionRep.GetValidByClientId(SelectedClient.Id).StartDate;
        ClientEndDate = subscriptionRep.GetValidByClientId(SelectedClient.Id).EndDate;
    }

    public void CloseAction(Action closeAction)
    {
        _closeAction = closeAction;
    }

    [RelayCommand]
    public void Cancel()
    {
        _closeAction();
    }
    
    [RelayCommand]
    public void Save()
    {
        if (SelectedClient.Name == "" || SelectedSchedule.Id < 1)
            return;
        ClientSubscription.StartDate = ClientStartDate;
        ClientSubscription.EndDate = ClientStartDate.AddDays(SelectedSchedule.Duration - 1);
        ClientSubscription.ClientId = SelectedClient.Id;
        ClientSubscription.ScheduleId = SelectedSchedule.Id;
        ClientSubscription.Validity = true;
        if (ClientSubscription.Id < 1)
        {
            _subscriptionRep.Insert(ClientSubscription);
            _closeAction();
            return;
        }
        _subscriptionRep.Update(ClientSubscription);
        _closeAction();
    }
    
    
}