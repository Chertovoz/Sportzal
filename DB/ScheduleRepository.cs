using System;
using System.Collections.Generic;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class ScheduleRepository: BaseRepository<Schedule>
{
    public ScheduleRepository(IOptions<DatabaseConnection> databaseConnection) : base(databaseConnection)
    {
        
    }
    
    public override List<Schedule> GetAll()
    {
        _connection.Open();
        List<Schedule> schedules = new List<Schedule>();
        string sql = "select * from schedule";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Schedule schedule = new Schedule();
            schedule.Id = reader.GetInt32("Id");
            schedule.Title = reader.GetString("Title");
            schedule.DaysAmount = reader.GetInt32("Days_amount");
            schedule.Duration = reader.GetInt32("Duration");
            schedule.Price= reader.GetInt32("Price");
                
            schedules.Add(schedule);
        }
        _connection.Close();
        return schedules;
    }

    public override Schedule GetById(int id)
    {
        _connection.Open();
        Schedule schedule = new Schedule();
        string sql = "Select s.Title, s.Days_amount, s.Duration, s.Price From schedule s where s.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            schedule.Title = reader.GetString("Title");
            schedule.DaysAmount = reader.GetInt32("Days_amount");
            schedule.Duration = reader.GetInt32("Duration");
            schedule.Price = reader.GetInt32("Price");
            schedule.Id = id;

        }
        _connection.Close();
        return schedule;
    }

    public override void Insert(Schedule entity)
    {
        _connection.Open();
        string sql = "Insert into schedule Values (0,@Title,@Days_amount,@Duration,@Price)";
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Title", entity.Title);
            mc.Parameters.AddWithValue("@Days_amount", entity.DaysAmount);
            mc.Parameters.AddWithValue("@Duration", entity.Duration);
            mc.Parameters.AddWithValue("@Price", entity.Price);
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();

    }

    public override void Update(Schedule entity)
    {
        _connection.Open();
        string sql="Update schedule Set Title = @Title, Days_amount = @Days_amount, Duration = @Duration, Price=@Price Where Id =" +  entity.Id;
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Title", entity.Title);
            mc.Parameters.AddWithValue("@Days_amount", entity.DaysAmount);
            mc.Parameters.AddWithValue("@Duration", entity.Duration);
            mc.Parameters.AddWithValue("@Price", entity.Price);
            mc.ExecuteNonQuery();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }

    public override void Delete(Schedule entity)
    {
        _connection.Open();
        string sql = "Delete From schedule Where Id ="+ entity.Id;
        try
        {
            using var mc =new MySqlCommand(sql, _connection);
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public Schedule GetByClientId(int id)
    {
        _connection.Open();
        Schedule schedule = new Schedule();
        string sql = "select s.Title, s.Days_amount, s.Price , s.Duration \n from schedule s \n inner join subscription s2 on s.Id = s2.Schedule_id \n inner join client c on c.Id = s2.Client_id where c.Id =  " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            schedule.Title = reader.GetString("Title");
            schedule.DaysAmount = reader.GetInt32("Days_amount");
            schedule.Duration = reader.GetInt32("Duration");
            schedule.Price = reader.GetInt32("Price");
            schedule.Id = id;
                
        }
        _connection.Close();
        return schedule;
    }
}