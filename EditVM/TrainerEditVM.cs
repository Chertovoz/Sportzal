using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;

namespace Gym_kursovaya.ViewModels;

public partial class TrainerEditVM: ViewModelBase
{
    [ObservableProperty] private Trainer _selectedTrainer;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _gender;
    [ObservableProperty] private int _price;
    private TrainerRepository _trainerRep;
     Action _closeAction;


    public TrainerEditVM(IServiceProvider serviceProvider,Trainer selectedTrainer, TrainerRepository trainerRepository)
    {
        _selectedTrainer = selectedTrainer;
        Name = selectedTrainer.Name;
        Gender = selectedTrainer.Gender;
        Price = selectedTrainer.Price;
        _trainerRep = trainerRepository;
    }
    
   
    
    
    [RelayCommand]
    public void Cancel()
    {
        _closeAction();
    }
    
    [RelayCommand]
    public void Save()
    {
        if (SelectedTrainer.Name == "" || SelectedTrainer.Price < 0)
            return;

        SelectedTrainer.Name = Name;
        SelectedTrainer.Price = Price;
        SelectedTrainer.Gender = Gender;

        if (SelectedTrainer.Id == 0)
        {
            _trainerRep.Insert(SelectedTrainer);
            _closeAction();
            return;
        }
        _trainerRep.Update(SelectedTrainer);
        _closeAction();
    }
    //закрытие окна
    public void CloseAction(Action action)
    {
        _closeAction =  action;
    }
    
    
    
}