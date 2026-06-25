using System.Collections.Generic;

namespace Gym_kursovaya.DB;

public interface IRepository<T> where T: class
{
    bool OpenConnection();
    bool CloseConnection();
    List<T> GetAll();
    T GetById(int id);
    public void Insert(T entity);
    public void Update(T entity);
    public void Delete(T entity);
}