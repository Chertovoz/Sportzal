using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public abstract class BaseRepository<T> : IDisposable, IRepository<T> where T : class
{
    protected MySqlConnection _connection;
    
    public abstract List<T> GetAll();
    
    public abstract T GetById(int id);
    
    public abstract void Insert(T entity);
    public abstract void Delete(T entity);
    public abstract void Update(T entity);
    public BaseRepository(IOptions<DatabaseConnection> options)
    {
        _connection = new MySqlConnection(options.Value.ConnectionString);
    }
    
    public void Dispose()
    {
        _connection.Dispose();
    }

    public bool OpenConnection()
    {
        try
        {
            _connection.Open();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool CloseConnection()
    {
        try
        {
            _connection.Close();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool ExecuteNonQuery(string query)
    {
        try
        {
            using (var command = new MySqlCommand(query, _connection))
                return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool ExecuteNonQuery(string query, MySqlParameter[] parameters)
    {
        try
        {
            using (var mc = new MySqlCommand(query, _connection))
            {
                mc.Parameters.AddRange(parameters);
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}