using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.ShowModels;
using Gym_kursovaya.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class SubscriptionVM : ViewModelBase
{
    [ObservableProperty] ObservableCollection<ShowSubscription> _subscriptions;
    [ObservableProperty] ShowSubscription _selectedSubscription;
    IServiceProvider _serviceProvider;
    SubscriptionRepository _subscriptionRep;
    ClientRepository  _clientRep;
    ScheduleRepository _scheduleRep;
    TrainerRepository _trainerRep;
    public SubscriptionVM(IServiceProvider serviceProvider, SubscriptionRepository subscriptionRepository, TrainerRepository trainerRepository, ScheduleRepository scheduleRepository, ClientRepository clientRepository)
    {
        
        _serviceProvider = serviceProvider;
        _subscriptionRep = subscriptionRepository;
        Subscriptions = new ObservableCollection<ShowSubscription>();
        _clientRep = clientRepository;
        _trainerRep = trainerRepository;
        _scheduleRep = scheduleRepository;
        FillList();
    }

    public void FillList()
    {
        foreach (var item in _subscriptionRep.GetAllTrainerSubscriptions())
        {
            ShowSubscription showSubscription = new ShowSubscription(item, _clientRep,_trainerRep, _scheduleRep);
            Subscriptions.Add(showSubscription);
        }
    }
    
    [RelayCommand]
    public void CreateSubscriptionWithTrainer()
    {
        var vm = ActivatorUtilities.CreateInstance<SybscriptionEditVM>(_serviceProvider,new Subscription());
        var win = _serviceProvider.GetRequiredService<EditSubscriptionWIndow>();
        win.DataContext = vm;
        win.Show();
    }
    
    [RelayCommand]
    public void EditSubscriptionWithTrainer()
    {
        var win = _serviceProvider.GetRequiredService<EditSubscriptionWIndow>();
        var vm = ActivatorUtilities.CreateInstance<SybscriptionEditVM>(_serviceProvider, SelectedSubscription.ChosenSubscription);
        win.DataContext = vm;
        win.Show();
    }
}