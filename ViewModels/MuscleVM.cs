using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class MuscleVM: ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Muscle> _showMuscles = new ObservableCollection<Muscle>();
    [ObservableProperty] private Muscle _selectedMuscle;
    private IServiceProvider _serviceProvider;
    private MuscleRepository _muscleRep;
    public MuscleVM(IServiceProvider serviceProvider, MuscleRepository muscleRepository)
    {
        _serviceProvider = serviceProvider;
             _muscleRep =  muscleRepository;
        ShowMuscles = new ObservableCollection<Muscle>(_muscleRep.GetAll());
        
    }

    [RelayCommand]
    public void AddMuscle()
    {
        var vm = ActivatorUtilities.CreateInstance<MuscleEditVM>(_serviceProvider, new Muscle());
        var win =  _serviceProvider.GetRequiredService<EditMuscleWindow>();
        win.DataContext = vm;
        vm.CloseAction(win.Close);
        win.Show();
        if (win == null)
        {
            ShowMuscles = new ObservableCollection<Muscle>(_muscleRep.GetAll());
        }
    }
    
    [RelayCommand]
    public void EditMuscle()
    {
        var vm = ActivatorUtilities.CreateInstance<MuscleEditVM>(_serviceProvider, SelectedMuscle);
        var win =  _serviceProvider.GetRequiredService<EditMuscleWindow>();
        win.DataContext = vm;
        vm.CloseAction(win.Close);
        win.Show();
        if (win == null)
        {
            ShowMuscles = new ObservableCollection<Muscle>(_muscleRep.GetAll());
        }
    }

    [RelayCommand]
    public void DeleteMuscle()
    {
        _muscleRep.Delete(SelectedMuscle);
        ShowMuscles =  new ObservableCollection<Muscle>(_muscleRep.GetAll());
    }
}