using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class MuscleProgramRepository: IDisposable
{
    protected MySqlConnection _connection;
    private MuscleRepository  _muscleRep;

    public MuscleProgramRepository(IOptions<DatabaseConnection> options,MuscleRepository muscleRepository)
    {
        _connection = new  MySqlConnection(options.Value.ConnectionString);
        _muscleRep = muscleRepository;
    }
    
    public void Dispose()
    {
        _connection.Dispose();
    }

    public void InsertListMuscles(Programm programm)
    {
        _connection.Open();
        int i = 0;
        string sql = $"Insert into muscle_program values (@{programm.Muscles[i].Id},@{programm.Id}) ";
        using var mc = new MySqlCommand(sql, _connection);
        try
        {
            while (i < programm.Muscles.Count)
            {
                mc.Parameters.AddWithValue("@"+programm.Muscles[i].Id, programm.Muscles[i].Id);
                mc.Parameters.AddWithValue("@" + programm.Id, programm.Id);
                mc.ExecuteNonQuery();
                i++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }

    public List<Muscle> GetMusclesByProgramId(int id)
    {
        _connection.Open();
        List<Muscle> muscles = new();
        string sql =
            " SELECT m.Id , m.Title \nFROM muscle m\ninner join muscle_program mp on m.Id = mp.Muscle_id \nwhere mp.Program_id = " +
            id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Muscle muscle = new();
            muscle.Id = reader.GetInt32("Id");
            muscle.Title = reader.GetString("Title");
            muscles.Add(muscle);
        }
        _connection.Close();
        return muscles;
    }

    public void DeleteMuscleListByProgramId(int id)
    {
        _connection.Open();
        string sql = "Delete  from muscle_program where m.Id =" + id;
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
        _connection.Close();
        return;
    }

    public void UpdateMusclesByProgram(Programm program)
    {
        if (program.Muscles.Any())
        { 
            _connection.Open();
        string sql1 = "Insert into muscle_program values ";
        string sql2 = "Delete From muscle_program where Program_id = "+ program.Id;
       
        using (var mc2 = new MySqlCommand(sql2, _connection))
            mc2.ExecuteNonQuery();

        
        StringBuilder sb = new(sql1);

            foreach (var muscle in program.Muscles)
            {
                sb.Append($"({muscle.Id}, {program.Id}),");
            }

            sb.Remove(sb.Length - 1, 1);

            using var mc1 = new MySqlCommand(sb.ToString(), _connection);
            mc1.ExecuteNonQuery();
            _connection.Close();
        }
        else
        {
            return;
        }
    }
}