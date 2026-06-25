using System;
using System.Collections.Generic;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class TrainerRepository: BaseRepository<Trainer>
{
    public TrainerRepository(IOptions<DatabaseConnection> databaseConnection) : base(databaseConnection)
    {
        
    }
    
    public override List<Trainer> GetAll()
    {
        _connection.Open();
        List<Trainer> trainers = new List<Trainer>();
        string sql = "select * from trainer";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Trainer trainer = new Trainer();
            trainer.Id = reader.GetInt32("Id");
            trainer.Name = reader.GetString("Name");
            trainer.Gender = reader.GetString("Gender");
            trainer.Price = reader.GetInt32("Price");
            trainers.Add(trainer);
        }
        _connection.Close();
        return trainers;
    }

    public override Trainer GetById(int id)
    {
        _connection.Open();
        Trainer trainer = new Trainer();
        string sql = "Select t.Name, t.Gender, t.Price From trainer t where t.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            trainer.Name = reader.GetString("Name");
            trainer.Gender = reader.GetString("Gender");
            trainer.Price = reader.GetInt32("Price");
            trainer.Id = id;

        }
        _connection.Close();
        return trainer;
    }

    public override void Insert(Trainer entity)
    {
        _connection.Open();
        string sql = "Insert into trainer Values (@Name,@Gender,@Price,0)";
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Name", entity.Name);
            mc.Parameters.AddWithValue("@Gender", entity.Gender);
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

    public override void Update(Trainer entity)
    {
        _connection.Open();
        string sql="Update trainer Set Name = @Name, Gender = @Gender, Price = @Price Where Id =" +  entity.Id;
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Name", entity.Name);
            mc.Parameters.AddWithValue("@Gender", entity.Gender);
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

    public override void Delete(Trainer entity)
    {
        _connection.Open();
        string sql = "Delete From trainer Where Id ="+ entity.Id;
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