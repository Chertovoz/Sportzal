using System;
using System.Collections.Generic;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class ExerciseStatusRepository: BaseRepository<ExerciseStatus>
{
    public ExerciseStatusRepository(IOptions<DatabaseConnection> databaseConnection) : base(databaseConnection)
    {
        
    }
    
    public override List<ExerciseStatus> GetAll()
    {
        _connection.Open();
        List<ExerciseStatus> day_statuses = new List<ExerciseStatus>();
        string sql = "select * from exercise_status";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            ExerciseStatus exerciseStatus = new ExerciseStatus();
            exerciseStatus.Id = reader.GetInt32("Id");
            exerciseStatus.Title = reader.GetString("Title");
            day_statuses.Add(exerciseStatus);
        }
        _connection.Close();
        return day_statuses;
    }

    public override ExerciseStatus GetById(int id)
    {
        _connection.Open();
        ExerciseStatus exerciseStatus = new ExerciseStatus();
        string sql = "Select ds.Title From exercise_status ds where ds.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            exerciseStatus.Id = id;
            exerciseStatus.Title = reader.GetString("Title");
        }
        _connection.Close();
        return exerciseStatus;
    }

    public override void Insert(ExerciseStatus entity)
    {
        _connection.Open();
        string sql = "Insert into exercise_status Values (0,@Title)";
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Title", entity.Title);
            
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();

    }

    public override void Update(ExerciseStatus entity)
    {
        _connection.Open();
        string sql="Update exercise_status Set Title = @Title Where Id =" +  entity.Id;
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Title", entity.Title);
            mc.ExecuteNonQuery();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }

    public override void Delete(ExerciseStatus entity)
    {
        _connection.Open();
        string sql = "Delete From exercise_status Where Id ="+ entity.Id;
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