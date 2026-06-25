using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class TrainerVM: ViewModelBase
{
    [ObservableProperty] Trainer _selectedTrainer;
    [ObservableProperty] ObservableCollection<Trainer>  _trainersCollection ;
    private IServiceProvider _serviceProvider;
    private TrainerRepository _trainerRep;
    public TrainerVM(TrainerRepository trainerRepository, IServiceProvider serviceProvider, Trainer?  selectedTrainer)
    {
        _serviceProvider = serviceProvider;
        _trainerRep = trainerRepository;
        _trainersCollection = new ObservableCollection<Trainer>(trainerRepository.GetAll());
        SelectedTrainer = selectedTrainer;
    }

    
    
    [RelayCommand]
    public void AddTrainer()
    {
        var vm = ActivatorUtilities.CreateInstance<TrainerEditVM>(_serviceProvider, new Trainer());
        var win = _serviceProvider.GetRequiredService<EditTrainerWindow>();
        win.DataContext = vm;
        vm.CloseAction(win.Close);
        win.Show();
    }

    [RelayCommand]
    public void EditTrainer()
    {
        if (SelectedTrainer == null)
            return;
        
        var vm = ActivatorUtilities.CreateInstance<TrainerEditVM>(_serviceProvider, SelectedTrainer);
        var win = _serviceProvider.GetRequiredService<EditTrainerWindow>();
        win.DataContext = vm;
        vm.CloseAction(win.Close);
        win.Show();
    }

    [RelayCommand]
    public void DeleteTrainer()
    {
        if (SelectedTrainer == null)
            return;
        
        _trainerRep.Delete(SelectedTrainer);
    }
}