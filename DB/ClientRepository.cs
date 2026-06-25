using System;
using System.Collections.Generic;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class ClientRepository: BaseRepository<Client>
{
    public ClientRepository(IOptions<DatabaseConnection> databaseConnection) : base(databaseConnection)
    {
        
    }

    public List<Client> GetNonValidClients()
    {
        _connection.Open();
        List<Client> clients= new List<Client>();
        string sql = "SELECT c.*\nFROM Client c\nWHERE NOT EXISTS\n(\n    SELECT 1\n    FROM Subscription s\n    WHERE s.Client_id  = c.Id\n      AND s.Validity = 1\n);";
        using var mc = new MySqlCommand(sql, _connection) ;
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Client client = new Client();
            client.Id = reader.GetInt32("Id");
            client.Name = reader.GetString("Name");
            client.Gender = reader.GetString("Gender");
            clients.Add(client);
        }
        _connection.Close();
        return clients;

    }
    
    public List<Client> GetValidClients()
    {
        _connection.Open();
        List<Client> clients= new List<Client>();
        string sql = "Select c.Id, c.Name, c.Gender\nFrom Client c\ninner join subscription s on s.Client_id = c.Id \nwhere s.Validity = 1";
        using var mc = new MySqlCommand(sql, _connection) ;
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Client client = new Client();
            client.Id = reader.GetInt32("Id");
            client.Name = reader.GetString("Name");
            client.Gender = reader.GetString("Gender");
            clients.Add(client);
        }
        _connection.Close();
        return clients;

    }

    public Client GetByClientName(string clientName)
    {
        _connection.Open();
        Client client = new Client();
        string sql = $"Select c.Id, c.Name, c.Gender From client c where c.Name = '{clientName}'";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            client.Id = reader.GetInt32("Id");
            client.Name = reader.GetString("Name");
            client.Gender= reader.GetString("Gender");
        }
        _connection.Close();
        return client;
    }
    public override List<Client> GetAll()
    {
        _connection.Open();
        List<Client> clients = new List<Client>();
        string sql = "select * from client";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Client client = new Client();
            client.Id = reader.GetInt32("Id");
            client.Name = reader.GetString("Name");
            client.Gender= reader.GetString("Gender");
            clients.Add(client);
        }
        _connection.Close();
        return clients;
    }

    public override Client GetById(int id)
    {
        _connection.Open();
        Client client = new Client();
        string sql = "Select c.Id, c.Name, c.Gender From client c where c.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            client.Id = reader.GetInt32("Id");
            client.Name = reader.GetString("Name");
            client.Gender = reader.GetString("Gender");
        }
        _connection.Close();
        return client;
    }

    public override void Insert(Client entity)
    {
        _connection.Open();
        string sql = "Insert into client Values (0,@Name, @Gender)";
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Name", entity.Name);
            mc.Parameters.AddWithValue("@Gender", entity.Gender);
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();

    }

    public override void Update(Client entity)
    {
        _connection.Open();
        string sql="Update client Set Name = @Name, Gender=@Gender Where Id =" +  entity.Id;
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Name", entity.Name);
            mc.Parameters.AddWithValue("@Gender", entity.Gender);
            mc.ExecuteNonQuery();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }

    public override void Delete(Client entity)
    {
        _connection.Open();
        string sql = "Delete From client Where Id ="+ entity.Id;
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
    }
}