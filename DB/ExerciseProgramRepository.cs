using System;
using System.Collections.Generic;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class ExerciseProgramRepository: IDisposable
{
    
    
    protected MySqlConnection _connection;
ExerciseStatusRepository _exerciseStatusRep;
    public ExerciseProgramRepository(IOptions<DatabaseConnection> options, ExerciseStatusRepository statusRepository) 
    {
        _connection = new MySqlConnection(options.Value.ConnectionString);
        _exerciseStatusRep = statusRepository;
    }

    public void InsertProgramms(Exercise entity)
    {
        _connection.Open();
        string sql = "Insert into exercise_program Values (@Program_id, @Exercise_id)";
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            foreach (Programm p in entity.Programms)
            {
                mc.Parameters.AddWithValue("@Program_id", p.Id);
                mc.Parameters.AddWithValue("@Exercise_id", entity.Id);
                mc.ExecuteNonQuery();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }
    
    public List<Programm> GetByExerciseId(int exerciseId)
    {
        _connection.Open();
        List<Programm> programs = new List<Programm>();
        string sql = "Select p.Id, p.Title, p.Description From exercise_program ep inner join program p on p.Id = ep.Program_id where ep.Id = " +  exerciseId;
        try
        {
            using var mc =  new MySqlCommand(sql, _connection);
            using var reader = mc.ExecuteReader();
            while (reader.Read())
            {
                Programm program = new Programm();
                program.Id = reader.GetInt32("Id");
                program.Title = reader.GetString("Title");
                program.Description = reader.GetString("Description");
                programs.Add(program);
            }
            _connection.Close();
            return programs;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<Exercise> GetByProgramId(int programId)
    {
        _connection.Open();
        List<Exercise> exercises = new List<Exercise>();
        string sql = "select d.Id, d.Date, d.Status_id, d.Subscription_Id from exercise d inner join exercise_program ep on  d.Id = ep.Program_id where ep.Program_id = " + programId;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Exercise exercise = new Exercise();
            exercise.Id = reader.GetInt32("Id");
            exercise.Date = reader.GetDateTime("Date");
            exercise.Status = _exerciseStatusRep.GetById(reader.GetInt32("Status_id"));
            exercise.SubscriptionId = reader.GetInt32("Subscription_Id");
            exercises.Add(exercise);
        }
        _connection.Close();
        return exercises;
    }
    
    public List<ExerciseProgramm> GetAll()
    {
        _connection.Open();
        List<ExerciseProgramm> exerciseProgramms = new List<ExerciseProgramm>();
        string sql = "Select * From exercise_program";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        try
        {
            while (reader.Read())
            {
                ExerciseProgramm exerciseProgramm = new ExerciseProgramm();
                exerciseProgramm.DayId = reader.GetInt32("day_id");
                exerciseProgramm.ProgrammId = reader.GetInt32("program_id");
                exerciseProgramms.Add(exerciseProgramm);
            }
            
                
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
        return exerciseProgramms;
    }

    public void DeleteByExerciseId(int exerciseId)
    {
        _connection.Open();
        string sql = "Delete * From exercise_program Where Id = "+ exerciseId;
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
    public void Dispose()
    {
        _connection.Dispose();
    }
    
    
}