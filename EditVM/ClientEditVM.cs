using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class ClientEditVM: ViewModelBase  
{
    //переданный клиент И его параметры
    [ObservableProperty] private Client  _selectedClient;
    [ObservableProperty] private string _clientName;
    [ObservableProperty] private Trainer _selectedTrainer;
    [ObservableProperty] private string _gender;
    [ObservableProperty] private Schedule _scheduleClient;
    //листы для комбобоксов
    [ObservableProperty] private List<Trainer> _trainers;
    [ObservableProperty] private List<Schedule> _schedules;
    private ClientRepository _clientRepository;
    private SubscriptionRepository _subscriptionRepository;
    Action _closeAction;
    IServiceProvider _serviceProvider;
    
    public ClientEditVM(IServiceProvider serviceProvider,Client selectedClient,  SubscriptionRepository subscriptionRep, ClientRepository clientRep, TrainerRepository trainerRep, ScheduleRepository scheduleRep)
    {
        _clientRepository = clientRep;
        SelectedClient =  selectedClient;
        Trainers = trainerRep.GetAll();
        Schedules = scheduleRep.GetAll();
        SelectedTrainer = trainerRep.GetById(subscriptionRep.GetById(selectedClient.Id).TrainerId);
        Gender = selectedClient.Gender;
        ClientName = selectedClient.Name;
        ScheduleClient = scheduleRep.GetById(subscriptionRep.GetById(selectedClient.Id).ScheduleId);
        _serviceProvider =  serviceProvider;
        _subscriptionRepository = subscriptionRep;
    }

    [RelayCommand]
    public void Cancel()
    {
        _closeAction();
    }

    /*
    [RelayCommand]
    public void CreateSubscription()
    {
        if (string.IsNullOrEmpty(ClientName) || SelectedTrainer.Id == 0 || ScheduleClient.Id == 0)
            return;
        
        var vm = ActivatorUtilities.CreateInstance<NoTrainerSubscriptionEditVM>(_serviceProvider, SelectedClient);
        var win = _serviceProvider.GetRequiredService<NoTrainerAbonementWindow>();
        win.DataContext = vm;
        win.Show();
    }
    */
/*
    [RelayCommand]
    public void CreateSubscription()
    {
        if (string.IsNullOrEmpty(ClientName) || SelectedTrainer.Id == 0 || ScheduleClient.Id == 0)
            return;
        
        Subscription sub = _subscriptionRepository.GetByClientId(SelectedClient.Id);
        if (SelectedTrainer.Name != "")
        {
            if (sub.ClientId != 0)
            {
                var vm = ActivatorUtilities.CreateInstance<SybscriptionEditVM>(_serviceProvider,sub, ClientName, SelectedTrainer.Name, ScheduleClient.Title);
                var win = _serviceProvider.GetRequiredService<EditSubscriptionWIndow>();
                win.DataContext = vm;
                win.Show();
            }
            else
            {
                DateTime end = DateTime.Now.AddDays(ScheduleClient.DaysAmount);
                sub = new Subscription
                { 
                    ClientId = _clientRepository.GetByClientName(ClientName).Id,
                    ScheduleId = ScheduleClient.Id,
                    TrainerId = SelectedTrainer.Id,
                    Validity = false,
                    StartDate = DateTime.Now, 
                    EndDate = end
                };
                var vm = ActivatorUtilities.CreateInstance<SybscriptionEditVM>(_serviceProvider,sub, ClientName, SelectedTrainer.Name, ScheduleClient.Title);
                var win = _serviceProvider.GetRequiredService<EditSubscriptionWIndow>();
                win.DataContext = vm;
                win.Show();
            }
        }
        else
        {
            
                var vm = ActivatorUtilities.CreateInstance<NoTrainerSubscriptionEditVM>(_serviceProvider, SelectedClient);
                var win = _serviceProvider.GetRequiredService<NoTrainerAbonementWindow>();
                win.DataContext = vm;
                win.Show();
            
        }
        
    }
    */

    //Доделать абонемент потом
    [RelayCommand]
    public void Save()
    {
        if(ClientName == "" && Gender == "" )
            return;

        SelectedClient.Gender = Gender;
        SelectedClient.Name = ClientName;
        if (SelectedClient.Id == 0)
        {
            _clientRepository.Insert(SelectedClient);
            _closeAction();
            return;
        }
        _clientRepository.Update(SelectedClient);
        _closeAction();
    }
    //закрытие окна
    public void CloseAction(Action action)
    {
        _closeAction =  action;
    }
}