

namespace APass.Core.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Add(T entity);
        void Edit(T entity);
        bool Remove(int id);
    }
}
