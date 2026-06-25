using System.Collections.ObjectModel;

namespace Gym_kursovaya.Models;

public class Programm
{
    public int Id{get;set;}
    public string Title{get;set;}
    public string Description{get;set;}
    public ObservableCollection<Muscle> Muscles{get;set;}
}