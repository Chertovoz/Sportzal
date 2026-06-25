using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;

namespace Gym_kursovaya.ViewModels;

public partial class MuscleEditVM: ViewModelBase
{
    [ObservableProperty] private Muscle _selectedMuscle;
    [ObservableProperty] private string _title;
    private MuscleRepository _muscleRep;
    
    
    public MuscleEditVM(Muscle selectedMuscle,  MuscleRepository muscleRepository)
    {
        SelectedMuscle = selectedMuscle;
        Title = SelectedMuscle.Title;
        _muscleRep = muscleRepository;
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

        SelectedMuscle.Title = Title;
        if (SelectedMuscle.Id == 0)
        {
            _muscleRep.Insert(SelectedMuscle);
            _closeAction();
            return;
        }
        _muscleRep.Update(SelectedMuscle);
        _closeAction();
    }
    //закрытие окна
    public void CloseAction(Action action)
    {
        _closeAction =  action;
    }
    
}