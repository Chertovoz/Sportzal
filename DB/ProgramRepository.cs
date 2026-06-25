using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gym_kursovaya.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Gym_kursovaya.DB;

public class ProgrammRepository: BaseRepository<Programm>

{
    private MuscleProgramRepository _muscleProgramRep;
    public ProgrammRepository(IOptions<DatabaseConnection> databaseConnection, MuscleProgramRepository muscleProgramRepository) : base(databaseConnection)
    {
        _muscleProgramRep = muscleProgramRepository;
    }
    
    public override List<Programm> GetAll()
    {
        _connection.Open();
        List<Programm> programs = new List<Programm>();
        string sql = "select * from program";
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        while (reader.Read())
        {
            Programm program = new Programm();
            program.Id = reader.GetInt32("Id");
            program.Title = reader.GetString("Title");
            program.Description = reader.GetString("Description");
            program.Muscles = new ObservableCollection<Muscle>(_muscleProgramRep.GetMusclesByProgramId(program.Id)); 
            programs.Add(program);
        }
        _connection.Close();
        return programs;
    }

    public override Programm GetById(int id)
    {
        _connection.Open();
        Programm programm = new Programm();
        string sql = "Select p.Title, p.Description From program p where p.Id = " + id;
        using var mc = new MySqlCommand(sql, _connection);
        using var reader = mc.ExecuteReader();
        if (reader.Read())
        {
            programm.Title = reader.GetString("Title");
            programm.Description = reader.GetString("Description");
            programm.Id = id;
            programm.Muscles = new ObservableCollection<Muscle>(_muscleProgramRep.GetMusclesByProgramId(programm.Id));
        }
        _connection.Close();
        return programm;
    }

    public override void Insert(Programm entity)
    {
        _connection.Open();
        string sql = "Insert into program Values (0,@Title, @Description)";
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Title", entity.Title);
            mc.Parameters.AddWithValue("@Description", entity.Description);
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();

    }

    public override void Update(Programm entity)
    {
        _connection.Open();
        string sql="Update program Set Title = @Title, Description = @Description Where Id =" +  entity.Id;
        try
        {
            using var mc = new MySqlCommand(sql, _connection);
            mc.Parameters.AddWithValue("@Title", entity.Title);
            mc.Parameters.AddWithValue("@Description", entity.Description);
            mc.ExecuteNonQuery();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _connection.Close();
    }

    //здесь также удаляется и все данные в кросс связи
    public override void Delete(Programm entity)
    {
        _connection.Open();
        string sql = "Delete From program Where Id ="+ entity.Id;
        try
        {
            using var mc =new MySqlCommand(sql, _connection);
            _muscleProgramRep.DeleteMuscleListByProgramId(entity.Id);
            mc.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<Programm> GetTodayClientId(int clientId)
    {
        _connection.Open();
        List<Programm> programs = new List<Programm>();
        string sql = "Select m.Title , m.Description, m.Id \nFrom program m\ninner join exercise_program ep on m.Id = ep.Program_id\ninner join exercise e on ep.Exercise_id = e.Id\ninner join subscription s on e.Subscription_id = s.Id\n inner join client c on c.Id = " + clientId +"  where e.Date = CURDATE();";
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
                program.Muscles = new ObservableCollection<Muscle>(_muscleProgramRep.GetMusclesByProgramId(program.Id));
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
    
    
    
}