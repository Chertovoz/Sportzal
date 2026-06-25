using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;

namespace Gym_kursovaya.ViewModels;

public partial class ProgrammEditVM : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Muscle> _muscles;
    [ObservableProperty] private Programm _selectedProgramm;
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _description;
    [ObservableProperty]  private ObservableCollection<Muscle> _selectedMuscles;
    private ProgrammRepository _programRep;
    private MuscleProgramRepository _muscleProgramRep;
    
    public ProgrammEditVM(Programm selectedProgramm, ProgrammRepository programRep, MuscleProgramRepository muscleProgramRep, MuscleRepository muscleRep)
    {
       _programRep = programRep;
       _muscleProgramRep = muscleProgramRep;
       SelectedProgramm =  selectedProgramm;
       Muscles = new ObservableCollection<Muscle>(muscleRep.GetAll());
       Title = selectedProgramm.Title;
       Description = selectedProgramm.Description;
       SelectedMuscles = selectedProgramm.Muscles;
    }
    
    Action _closeAction;
    [RelayCommand]
    public void Cancel()
    {
        _closeAction();
    }
    
    [RelayCommand]
    public void Save()
    {
        if (Title == "")
            return;
        
        SelectedProgramm.Title = Title;
        SelectedProgramm.Description = Description;
        SelectedProgramm.Muscles = new ObservableCollection<Muscle>(SelectedMuscles);
        if (SelectedProgramm.Id == 0)
        {
            _programRep.Insert(SelectedProgramm);
            _muscleProgramRep.InsertListMuscles(SelectedProgramm);
            _closeAction();
            return;
        }
        _programRep.Update(SelectedProgramm);
        _muscleProgramRep.UpdateMusclesByProgram(SelectedProgramm);
        _closeAction();
    }
    //закрытие окна
    public void CloseAction(Action action)
    {
        _closeAction =  action;
    }
    
    
    
}