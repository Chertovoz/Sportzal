using System;
using System.Collections.Generic;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class SubscriptionRepository: BaseRepository<Subscription>
{
    public SubscriptionRepository(IOptions<DatabaseConnection> databaseConnection) : base(databaseConnection)
    {
        
    }
    
    public override List<Subscription> GetAll()
    {
        _connection.Open();
        List<Subscription> subscriptions = new List<Subscription>();
        string sql = "select * from subscription";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Subscription subscription = new Subscription();
            subscription.Id = reader.GetInt32("Id");
            subscription.ClientId = reader.GetInt32("Client_id");
            subscription.ScheduleId = reader.GetInt32("Schedule_id");
            subscription.TrainerId = reader.GetInt32("Trainer_id");
            subscription.EndDate = reader.GetDateTime("End_date");
            subscription.StartDate = reader.GetDateTime("Start_date");
            subscription.Validity = reader.GetBoolean("Validity");
            subscriptions.Add(subscription);
        }
        _connection.Close();
        return subscriptions;
    }

    public override Subscription GetById(int id)
    {
        _connection.Open();
        Subscription subscription = new Subscription();
        string sql = "Select s.Client_id, s.Schedule_id, s.Trainer_id, s.End_date, s.Start_date, s.Validity From subscription s where s.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            subscription.ClientId = reader.GetInt32("Client_id");
            subscription.ScheduleId = reader.GetInt32("Schedule_id");
            subscription.TrainerId = reader.GetInt32("Trainer_id");
            subscription.EndDate = reader.GetDateTime("End_date");
            subscription.StartDate = reader.GetDateTime("Start_date");
            subscription.Validity = reader.GetBoolean("Validity");
            subscription.Id = id;
        }
        _connection.Close();
        return subscription;
    }

    public override void Insert(Subscription entity)
    {
        _connection.Open();
        string sql = "Insert into subscription Values (0,@Client_id,@Schedule_id,@Trainer_id,@End_date,@Start_date,@Validity)";
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Client_id", entity.ClientId);
            mc.Parameters.AddWithValue("@Schedule_id", entity.ScheduleId);
            mc.Parameters.AddWithValue("@Trainer_id", entity.TrainerId);
            mc.Parameters.AddWithValue("@End_date", entity.EndDate);
            mc.Parameters.AddWithValue("@Start_date", entity.StartDate);
            mc.Parameters.AddWithValue("@Validity", entity.Validity);
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();

    }

    public override void Update(Subscription entity)
    {
        _connection.Open();
        string sql="Update subscription Set Client_id = @Client_id, Schedule_id= @Schedule_id, Trainer_id=@Trainer_id,End_date=@End_date,Start_date=@Start_date, Validity=@Validity   Where Id =" +  entity.Id;
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Client_id", entity.ClientId);
            mc.Parameters.AddWithValue("@Schedule_id", entity.ScheduleId);
            mc.Parameters.AddWithValue("@Trainer_id", entity.TrainerId);
            mc.Parameters.AddWithValue("@End_date", entity.EndDate);
            mc.Parameters.AddWithValue("@Start_date", entity.StartDate);
            mc.Parameters.AddWithValue("@Validity", entity.Validity);
            mc.ExecuteNonQuery();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }

    public override void Delete(Subscription entity)
    {
        _connection.Open();
        string sql = "Delete From subscription Where Id ="+ entity.Id;
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

    public Subscription GetByClientId(int id)
    {
        Subscription subscription = new Subscription();
        _connection.Open();
        string sql =
            "select s.Id, s.Client_id, s.Schedule_id, s.Trainer_id, s.End_date, s.Start_date, s.Validity from subscription s  where s.Validity = 1 and s.Client_id =   " +
                id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        try
        {
            if (reader.Read())
            {
                subscription.Id = reader.GetInt32("Id");
                subscription.ClientId = reader.GetInt32("Client_id");
                subscription.ScheduleId = reader.GetInt32("Schedule_id");
                subscription.TrainerId = reader.GetInt32("Trainer_id");
                subscription.EndDate = reader.GetDateTime("End_date");
                subscription.StartDate = reader.GetDateTime("Start_date");
                subscription.Validity = reader.GetBoolean("Validity");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
        return subscription;
    }
    
    public List<Subscription> GetAllByClientId(int clientId)
    {
        _connection.Open();
        List<Subscription> subscriptions =  new List<Subscription>();
        string sql =
            "Select s.Id, s.Client_id, s.Schedule_id, s.Trainer_id, s.End_date, s.Start_date, s.Validity\nFrom subscription s inner join client c on s.Client_id =" + clientId + "\nwhere s.Validity = 1";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        try
        {
            while (reader.Read())
            {
                Subscription subscription = new Subscription();
                subscription.ClientId = reader.GetInt32("Client_id");
                subscription.ScheduleId = reader.GetInt32("Schedule_id");
                subscription.TrainerId = reader.GetInt32("Trainer_id");
                subscription.EndDate = reader.GetDateTime("End_date");
                subscription.StartDate = reader.GetDateTime("Start_date");
                subscription.Validity = reader.GetBoolean("Validity");
                subscription.Id = reader.GetInt32("Id");
                subscriptions.Add(subscription);
            }
            _connection.Close();
            return subscriptions;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public int InsertIdBack(Subscription subscription)
    {
        _connection.Open();

        string sql = @"
        INSERT INTO subscription
        (
            Client_id,
            Trainer_id,
            Schedule_id,
            Start_date,
            End_date,
            Validity
        )
        VALUES
        (
            @ClientId,
            @TrainerId,
            @ScheduleId,
            @StartDate,
            @EndDate,
            @Validity
        );";

        using var cmd = new MySqlCommand(sql, _connection);

        cmd.Parameters.AddWithValue("@ClientId", subscription.ClientId);
        cmd.Parameters.AddWithValue("@TrainerId", subscription.TrainerId);
        cmd.Parameters.AddWithValue("@ScheduleId", subscription.ScheduleId);
        cmd.Parameters.AddWithValue("@StartDate", subscription.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", subscription.EndDate);
        cmd.Parameters.AddWithValue("@Validity", subscription.Validity);

        cmd.ExecuteNonQuery();

        int id = (int)cmd.LastInsertedId;

        _connection.Close();

        return id;
    }
    
    public Subscription GetValidByClientId(int clientId)
    {
        _connection.Open();
        Subscription  subscription = new Subscription();
        string sql ="Select s.Id, s.Client_id, s.Schedule_id, s.Trainer_id, s.End_date, s.Start_date, s.Validity From subscription s inner join client c on s.Client_id = " + clientId + " Where s.Validity = 1";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        try
        {
            if (reader.Read())
            {
                subscription.Id = reader.GetInt32("Id");
                subscription.ScheduleId = reader.GetInt32("Schedule_id");
                subscription.TrainerId = reader.GetInt32("Trainer_id");
                subscription.EndDate = reader.GetDateTime("End_date");
                subscription.StartDate = reader.GetDateTime("Start_date");
                subscription.Validity = reader.GetBoolean("Validity");
                subscription.ClientId = reader.GetInt32("Client_id");
            }
            _connection.Close();
            return subscription;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void ChangeValidityByClientId(int id)
    {
        _connection.Open();
        string sql = "update subscription s \nset validity = 0\nwhere s.Client_id = " + id;
        try
        {
            using var mc = new MySqlCommand(sql, _connection); 
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
        
    }

    public List<Subscription> GetAllTrainerSubscriptions()
    {
        _connection.Open();
        List<Subscription> subscriptions = new List<Subscription>();
        string sql = "SELECT *\nFROM subscription\nWHERE Trainer_id IS NOT NULL\nORDER BY Validity DESC, Id DESC;";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Subscription subscription = new Subscription();
            subscription.Id = reader.GetInt32("Id");
            subscription.ClientId = reader.GetInt32("Client_id");
            subscription.ScheduleId = reader.GetInt32("Schedule_id");
            subscription.TrainerId = reader.GetInt32("Trainer_id");
            subscription.EndDate = reader.GetDateTime("End_date");
            subscription.StartDate = reader.GetDateTime("Start_date");
            subscription.Validity = reader.GetBoolean("Validity");
            subscriptions.Add(subscription);
        }
        _connection.Close();
        return subscriptions;
    }
    
    public List<Subscription> GetAllNoTrainerSubscriptions()
    {
        _connection.Open();
        List<Subscription> subscriptions = new List<Subscription>();
        string sql = "SELECT *\nFROM subscription\nWHERE Trainer_id IS NULL\nORDER BY Validity DESC, Id DESC;";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Subscription subscription = new Subscription();
            subscription.Id = reader.GetInt32("Id");
            subscription.ClientId = reader.GetInt32("Client_id");
            subscription.ScheduleId = reader.GetInt32("Schedule_id");
            subscription.TrainerId = reader.GetInt32("Trainer_id");
            subscription.EndDate = reader.GetDateTime("End_date");
            subscription.StartDate = reader.GetDateTime("Start_date");
            subscription.Validity = reader.GetBoolean("Validity");
            subscriptions.Add(subscription);
        }
        _connection.Close();
        return subscriptions;
    }
}