using System;

namespace Gym_kursovaya.Models;

public class Subscription
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int TrainerId { get; set; }
    public int ScheduleId { get; set; }
    public bool Validity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
}
