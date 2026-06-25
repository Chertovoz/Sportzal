using System;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.ViewModels;

namespace Gym_kursovaya.ShowModels;

public class ShowSubscription: ViewModelBase
{
    public ShowSubscription(Subscription subscription,ClientRepository clientRepository,  TrainerRepository trainerRepository, ScheduleRepository scheduleRepository)
    {
        ChosenSubscription = subscription;
        SClient = clientRepository.GetById(ChosenSubscription.ClientId);
        STrainer = trainerRepository.GetById(ChosenSubscription.TrainerId);
        SSchedule = scheduleRepository.GetById(ChosenSubscription.ScheduleId);
    }
    public Subscription ChosenSubscription { get; set; }
    public Client SClient{ get; set; }
    public Trainer STrainer{ get; set; }
    public Schedule SSchedule{ get; set; }
}