using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;

namespace Gym_kursovaya.ViewModels;

//здесь работа окна отмечания клиентов
public partial class MarkClientsVM: ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Client> _clients;
    [ObservableProperty]private ObservableCollection<Client> _selectedClients;
    private SubscriptionRepository _subscriptionRep;
    private ExerciseRepository _exerciseRep;
    private ClientRepository _clientRep;
    private ScheduleRepository _scheduleRep;
    public MarkClientsVM(IServiceProvider serviceProvider,ClientRepository clientRepository, SubscriptionRepository subscriptionRepository, ExerciseRepository exerciseRepository, ScheduleRepository scheduleRepository)
    {
        _clientRep =  clientRepository;
        GetClientsList();
       _subscriptionRep = subscriptionRepository;
       _exerciseRep = exerciseRepository;
       _scheduleRep = scheduleRepository;
    }

    //здесь получаем клиентов с валидной подпиской и если статус дня уже не завершен или не выходной.
    //А также если тренировки вообще нет
    public void GetClientsList()
    {
        foreach (var client in _clientRep.GetValidClients())
        {
            if (_exerciseRep.GetTodayByClientId(client.Id).Status.Id != 1 &&
                _exerciseRep.GetTodayByClientId(client.Id).Status.Id > 0
                && _exerciseRep.GetTodayByClientId(client.Id).Status.Id != 2)
            {
                Clients.Add(client);
            }
        }
        
    }
    
    //здесь мы подтверждаем и  одновременно меняем валидность подписки, есил условия прошли
    [RelayCommand]
    public void Confirm()
    {
        foreach (var client in SelectedClients)
        {
            if (_exerciseRep.GetTodayByClientId(client.Id).Id > 0)
            {
                Exercise exercise = _exerciseRep.GetTodayByClientId(client.Id);
                exercise.Status.Id = 1;
                _exerciseRep.Update(exercise);
                int i = 0;
                foreach (var ex in _exerciseRep.GetBySubscriptionId(_subscriptionRep.GetByClientId(client.Id).Id))
                {
                    if (ex.Status.Id == 1)
                    {
                        i++;
                    }
                }
                if (i < _scheduleRep.GetByClientId(client.Id).DaysAmount)
                {
                   Subscription subscription =  _subscriptionRep.GetByClientId(client.Id);
                   subscription.Validity = false;
                   _subscriptionRep.Update(subscription);
                }
            }
            else
            {
                Exercise exercise = new Exercise();
                exercise.Status.Id = 1;
                exercise.Date =  DateTime.Today;
                exercise.SubscriptionId = _subscriptionRep.GetByClientId(client.Id).Id;
                    _exerciseRep.Insert(exercise);
                    int i = 0;
                    foreach (var ex in _exerciseRep.GetBySubscriptionId(_subscriptionRep.GetByClientId(client.Id).Id))
                    {
                        if (ex.Status.Id == 1)
                        {
                            i++;
                        }
                    }
                    if (i < _scheduleRep.GetByClientId(client.Id).DaysAmount)
                    {
                        Subscription subscription =  _subscriptionRep.GetByClientId(client.Id);
                        subscription.Validity = false;
                        _subscriptionRep.Update(subscription);
                    }
            }
        }
    }
    
}