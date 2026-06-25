using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class ProgramVM: ViewModelBase
{
    [ObservableProperty]private ObservableCollection<Programm> _programs;
    [ObservableProperty]private Programm _selectedProgram;
    [ObservableProperty]private ObservableCollection<Muscle> _muscles;
    [ObservableProperty]private ObservableCollection<Muscle> _selectedMuscles;
    private MuscleRepository _muscleRep;
     
    private ProgrammRepository _programmRep;
    IServiceProvider _serviceProvider;
    
    
    public ProgramVM(IServiceProvider provider, ProgrammRepository programmRepository)
    {
        _serviceProvider = provider;
        _programmRep = programmRepository;
        _programs = new ObservableCollection<Programm>(_programmRep.GetAll());
        
    }

    [RelayCommand]
    public void AddProgram()
    {
        var vm = ActivatorUtilities.CreateInstance<ProgrammEditVM>(_serviceProvider, new Programm());
        var win = _serviceProvider.GetRequiredService<EditProgrammWindow>();
        win.DataContext = vm;
        vm.CloseAction(win.Close);
        win.Show();
    }

    [RelayCommand]
    public void EditProgram()
    {
        var vm = ActivatorUtilities.CreateInstance<ProgrammEditVM>(_serviceProvider, SelectedProgram);
        var win = _serviceProvider.GetRequiredService<EditProgrammWindow>();
        win.DataContext = vm;
        vm.CloseAction(win.Close);
        win.Show();
    }

    [RelayCommand]
    public void DeleteProgram()
    {
        _programmRep.Delete(SelectedProgram);
    }
    
}