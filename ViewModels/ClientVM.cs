using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.ShowModels;
using Gym_kursovaya.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class ClientVM: ViewModelBase
{
    //все клиенты
     List<Client> _allClients;
    //список клиентов которые будут показываться в таблице (создан спец класс, чтобы отобраались другие параметры)
    [ObservableProperty] private ObservableCollection<ShowClient> _showClients = new ObservableCollection<ShowClient>(); 
    //список выбранных клиентов, которые были на тренировке.
    [ObservableProperty]  private List<Client> _appearedClients = new List<Client>();
    //Клиент на редактирование
    [ObservableProperty] private ShowClient _selectedClient;
    //Список фильтрованных клиентов, которые будут переделаны в _showClients
    private List<Client> _filteredClients;
    
    ClientRepository _clientRepository;
    SubscriptionRepository _subscriptionRepository;
    ScheduleRepository _scheduleRepository;
    ProgrammRepository _programmRepository;
    IServiceProvider _serviceProvider;
    
    
    public ClientVM(IServiceProvider serviceProvider, ClientRepository clientRep, SubscriptionRepository subscriptionRep, ScheduleRepository scheduleRep, ProgrammRepository programmRep)
    {       
        _clientRepository= clientRep;
        _allClients = clientRep.GetAll();
        _subscriptionRepository = subscriptionRep;
        _scheduleRepository = scheduleRep;
        _programmRepository = programmRep;
        _serviceProvider = serviceProvider;
        _filteredClients= _allClients.OrderBy(x => x.Name).ToList();
        FillShowClients();
    }

    [RelayCommand]
    public void EditClient()
    {
        var vm = ActivatorUtilities.CreateInstance<ClientEditVM>(_serviceProvider, _clientRepository.GetById(SelectedClient.ClientId));
        var win = _serviceProvider.GetRequiredService<EditClientWindow>();
        vm.CloseAction(win.Close);
        win.DataContext = vm;
        win.Show();
        
    }
    
    [RelayCommand]
    public void CreateClient()
    {
        var vm = ActivatorUtilities.CreateInstance<ClientEditVM>(_serviceProvider, new Client());
        var win = _serviceProvider.GetRequiredService<EditClientWindow>();
        win.DataContext = vm;
        vm.CloseAction(win.Close);
        win.Show();
        
    }

    [RelayCommand]
    public void DeleteClient()
    {
        _clientRepository.Delete(_clientRepository.GetById(SelectedClient.ClientId));
    }
    
    public void FillShowClients()
    {
        foreach (var client in _filteredClients)
        {
            ShowClient showClient = new ShowClient();
            showClient.ClientId =  client.Id;
            showClient.Name =  client.Name;
            showClient.Gender = client.Gender;
            showClient.ScheduleName = _scheduleRepository.GetByClientId(client.Id).Title;
            if (showClient.ScheduleName == "")
            {
                showClient.ScheduleName = "нет";
            }
            foreach (var program in _programmRepository.GetTodayClientId(client.Id))
            {
                showClient.ProgrammTitles += program.Title + ", ";
            }
            ShowClients.Add(showClient);
        }
    }
    
    [RelayCommand]
    public void SortByClientName()
    {
        _filteredClients.Clear();
        _filteredClients = _allClients.OrderBy(x => x.Name).ToList();
        FillShowClients();
    }
    
}