namespace UserAPI.Repositories
{
    public interface IRepository<T>
    {
            IEnumerable<T> Get();
            T Get(Guid key);
            bool Add(T entity);
            bool Update(T entity);
            bool Delete(object key);
        
    }
}
