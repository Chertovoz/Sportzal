using System;
using System.Collections.Generic;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class MuscleRepository: BaseRepository<Muscle>
{
    public MuscleRepository(IOptions<DatabaseConnection> databaseConnection) : base(databaseConnection)
    {
        
    }
    
    public override List<Muscle> GetAll()
    {
        _connection.Open();
        List<Muscle> muscles = new List<Muscle>();
        string sql = "select * from Muscle";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Muscle muscle = new Muscle();
            muscle.Id = reader.GetInt32("Id");
            muscle.Title = reader.GetString("Title");
            muscles.Add(muscle);
        }
        _connection.Close();
        return muscles;
    }

    public override Muscle GetById(int id)
    {
        _connection.Open();
        Muscle muscle = new Muscle();
        string sql = "Select m.Title From Muscle m where m.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            muscle.Title = reader.GetString("Title");
            muscle.Id = id;

        }
        _connection.Close();
        return muscle;
    }

    public override void Insert(Muscle entity)
    {
        _connection.Open();
        string sql = "Insert into Muscle Values (0,@Title)";
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

    public override void Update(Muscle entity)
    {
        _connection.Open();
        string sql="Update Muscle Set Title = @Title Where Id =" +  entity.Id;
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

    public override void Delete(Muscle entity)
    {
        _connection.Open();
        string sql = "Delete From Muscle Where Id ="+ entity.Id;
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