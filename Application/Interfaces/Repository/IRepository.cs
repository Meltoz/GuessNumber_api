using Shared;

namespace Application.Interfaces.Repository
{
    public interface IRepository<T>
    {
        public Task<IEnumerable<T>> GetAllAsync();

        public Task<T?> GetByIdAsync(Guid id);

        public T? GetById(Guid id);

        public T? Insert(T domain);

        public Task<T> InsertAsync(T domain);

        public Task InsertWithOutSave(T domain);

        public Task<T> UpdateAsync(T domain);

        public Task UpdateWithOutSaveAsync(T domain);

        public Task<T> UpsertAsync(T domain);

        public Task<T> UpsertWithOutSaveAsync(T domain);

        public void Delete(Guid id);

        public void Delete(T domain);

        public void Save();

        public Task SaveAsync();
    }
}
