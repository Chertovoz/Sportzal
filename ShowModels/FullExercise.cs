using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.ViewModels;

namespace Gym_kursovaya.ShowModels;

public partial class FullExercise : ViewModelBase
{
    public FullExercise(Exercise exercise)
    {
        ChosenExercise = exercise;

        Programs =
            new ObservableCollection<Programm>(
                exercise.Programms);
    }

    public Exercise ChosenExercise { get; }

    public bool IsCurrentMonth { get; set; }

    public bool IsEditable { get; set; }

    public int DayNumber => ChosenExercise.Date.Day;

    public DateTime Date => ChosenExercise.Date;

    public ExerciseStatus Status
    {
        get => ChosenExercise.Status;
        set
        {
            ChosenExercise.Status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public ObservableCollection<Programm> Programs { get; }
}