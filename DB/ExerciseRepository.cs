using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class ExerciseRepository: BaseRepository<Exercise>
{
    private ExerciseProgramRepository _exerciseProgramRep;
    private ExerciseStatusRepository _exerciseStatusRep;
     public ExerciseRepository(IOptions<DatabaseConnection> databaseConnection, ExerciseProgramRepository exerciseProgramRep, ExerciseStatusRepository exerciseStatusRep) : base(databaseConnection)
    {
        _exerciseProgramRep = exerciseProgramRep;
        _exerciseStatusRep = exerciseStatusRep;
    }

    public Exercise GetTodayByClientId(int clientId)
    {
        _connection.Open();
        Exercise exercise = new Exercise();
        string sql = "select e.`Date` , e.Id, e.Status_id , e.Subscription_id \nfrom exercise e \ninner join subscription s on s.Id = e.Subscription_id \nwhere e.`Date` = CURDATE() and s.Client_id = " +  clientId;
        
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            using var reader = mc.ExecuteReader();
            if (reader.Read())
            {
                exercise.Date = reader.GetDateTime("Date");
                exercise.Status = _exerciseStatusRep.GetById(reader.GetInt32("Status_id"));
                exercise.SubscriptionId = reader.GetInt32("Subscription_Id");
                exercise.Id = reader.GetInt32("Id");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        
        _connection.Close();
        return exercise;
    }
     
    public override List<Exercise> GetAll()
    {
        _connection.Open();
        List<Exercise> days = new List<Exercise>();
        string sql = "select * from client";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Exercise exercise = new Exercise();
            exercise.Id = reader.GetInt32("Id");
            exercise.Date = reader.GetDateTime("Date");
            exercise.Status = _exerciseStatusRep.GetById(reader.GetInt32("Status_id"));
            exercise.SubscriptionId = reader.GetInt32("Subscription_Id");
            //здесь находим все программы на день
            exercise.Programms=new List<Programm>(_exerciseProgramRep.GetByExerciseId(exercise.Id));
            days.Add(exercise);
        }
        _connection.Close();
        return days;
    }

    public override Exercise GetById(int id)
    {
        _connection.Open();
        Exercise exercise = new Exercise();
        string sql = "Select d.Id, d.Date, d.Status_id, d.Subscription_Id From exercise d where d.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            exercise.Id = reader.GetInt32("Id");
            exercise.Date = reader.GetDateTime("Date");
            exercise.Status = _exerciseStatusRep.GetById(reader.GetInt32("Status_id"));
            exercise.SubscriptionId = reader.GetInt32("Subscription_Id");
            exercise.Programms=new List<Programm>(_exerciseProgramRep.GetByExerciseId(exercise.Id));
        }
        _connection.Close();
        return exercise;
    }
    public override void Insert(Exercise entity)
    {
        _connection.Open();
        string sql1 = "Insert into exercise Values (0,@Status_id, @Subscription_id,@Date)";
        try
        {
            using var mc = new MySqlCommand(sql1, _connection);
            mc.Parameters.AddWithValue("@Status_id", entity.Status.Id);
            mc.Parameters.AddWithValue("@Subscription_Id", entity.SubscriptionId);
            mc.Parameters.AddWithValue("@Date", entity.Date);
            _exerciseProgramRep.InsertProgramms(entity);
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();

    }

    //просто два разных репозитория 
    public override void Update(Exercise entity)
    {
        _connection.Open();
        string sql1="Update Exercise Set Status_Id = @Status_id, Subscription_Id = @Subscription_id, Date = @Date Where Id =" +  entity.Id;
        string sql2 = "delete from exercise_program ep\nwhere ep.Exercise_id = " + entity.Id;
        string sql3 = "Insert into  exercise_program Values ";
        StringBuilder sb = new StringBuilder(sql3);
        entity.Programms.Select((s => sb.Append($"({s.Id}, {entity.Id}),")));
        sb.Remove(sb.Length - 1, 1);
        sql1 = sb.ToString();
        using var transaction = _connection.BeginTransaction();
        try
        {

            using (var mc = new MySqlCommand(sql1, _connection, transaction))
            {
                mc.Parameters.AddWithValue("@Status_id", entity.Status.Id);
                mc.Parameters.AddWithValue("@Subscription_id", entity.SubscriptionId);
                mc.Parameters.AddWithValue("@Date", entity.Date);
                mc.ExecuteNonQuery();
            }
            using (var mc = new MySqlCommand(sql2, _connection, transaction))
            {
                mc.ExecuteNonQuery();
            }

            using (var mc = new MySqlCommand(sql3, _connection, transaction))
            {
                mc.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }

    public override void Delete(Exercise entity)
    {
        _connection.Open();
        string sql = "Delete From exercise Where Id ="+ entity.Id;
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

    public List<Exercise> GetBySubscriptionId(int subscriptionId)
    {
        if (subscriptionId <= 0)
        {
            List<Exercise> list = new List<Exercise>();
                        return list;
        }
        
        _connection.Open();
        List<Exercise> days = new List<Exercise>();
        string sql =
            "Select d.Status_id, d.Subscription_id, d.Date From exercise d  inner join Subscription s on d.Subscription_id = " +
            subscriptionId;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Exercise exercise = new Exercise();
            exercise.Status = _exerciseStatusRep.GetById(reader.GetInt32("Status_id"));
            exercise.SubscriptionId = reader.GetInt32("Subscription_id");
            exercise.Date = reader.GetDateTime("Date");
            exercise.Id = reader.GetInt32("Id");
            exercise.Programms =  new List<Programm>(_exerciseProgramRep.GetByExerciseId(exercise.Id));
            days.Add(exercise);
        }
        _connection.Close();
        return days;
    }

    public void DeleteAllByClientId(int clientId)
    {
        _connection.Open();
        string sql = "Delete from exercise Where Client_id =" + clientId;
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
}