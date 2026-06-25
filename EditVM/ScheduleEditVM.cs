using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;

namespace Gym_kursovaya.ViewModels;

public partial class ScheduleEditVM: ViewModelBase
{
    [ObservableProperty] private Schedule _selectedSchedule;
    [ObservableProperty] private int _price;
    [ObservableProperty] private string _title;
    [ObservableProperty] private int _daysAmount;
    [ObservableProperty] private int _duration;
    ScheduleRepository _scheduleRep;
    Action _closeAction;
    public ScheduleEditVM(Schedule selectedSchedule, ScheduleRepository  scheduleRepository)
    {
        SelectedSchedule=selectedSchedule;
        _scheduleRep = scheduleRepository;
        Price = SelectedSchedule.Price;
        Title = SelectedSchedule.Title;
        DaysAmount = SelectedSchedule.DaysAmount;
        Duration = SelectedSchedule.Duration;
    }
    
    
    [RelayCommand]
    public void Cancel()
    {
        _closeAction();
    }
    
    [RelayCommand]
    public void Save()
    {
        if (string.IsNullOrEmpty(SelectedSchedule.Title)|| SelectedSchedule.DaysAmount <= 0 ||
            SelectedSchedule.Duration <= 0 || SelectedSchedule.Price <0)
            return;

        SelectedSchedule.Title = Title;
        SelectedSchedule.DaysAmount = DaysAmount;
        SelectedSchedule.Duration = Duration;
        SelectedSchedule.Price = Price;
        if (SelectedSchedule.Id == 0)
        {
            _scheduleRep.Insert(SelectedSchedule);
            _closeAction();
            return;
        }
        _scheduleRep.Update(SelectedSchedule);
        _closeAction();

    }
    //закрытие окна
    public void CloseAction(Action action)
    {
        _closeAction =  action;
    }
    
    
}