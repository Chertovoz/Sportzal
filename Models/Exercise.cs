using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gym_kursovaya.DB;

namespace Gym_kursovaya.Models;

public class Exercise
{
    public int Id{get;set;}
    public DateTime Date{get;set;}
    public ExerciseStatus Status{get;set;} 
    public int SubscriptionId{get;set;}
    public List<Programm> Programms { get; set; } = new List<Programm>();


}