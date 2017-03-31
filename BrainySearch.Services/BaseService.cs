namespace BrainySearch.Services
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Data.Entity;

    public interface IBaseService<T>
        where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(Guid id);

        Task<T> Create();
        Task Add(T item);
        Task Delete(T item);
        Task DeleteById(Guid id);
        Task Save();
    }

    public class BaseService<T> : IBaseService<T>
        where T : class, new()
    {
        protected readonly ApplicationDbContext applicationDbContext;

        public BaseService(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        virtual public async Task<List<T>> GetAll()
        {
            return await applicationDbContext.Set<T>().ToListAsync();
        }

        virtual public async Task<T> GetById(Guid id)
        {
            var propertyInfo = typeof(T).GetProperties()
                .FirstOrDefault(item => item.Name == "Id" && item.CanRead && item.PropertyType == typeof(Guid));

            if (propertyInfo == null) return null;
            else return await applicationDbContext.Set<T>().FirstOrDefaultAsync(item => (Guid)propertyInfo.GetValue(item) == id);
        }

        virtual public async Task<T> Create()
        {
            return await new Task<T>(() => new T());
        }

        virtual public async Task Add(T item)
        {
            await new Task(() => applicationDbContext.Set<T>().Add(item));
        }

        virtual public async Task Delete(T item)
        {
            await new Task(() => applicationDbContext.Set<T>().Remove(item));
        }

        virtual public async Task DeleteById(Guid id)
        {
            var propertyInfo = typeof(T).GetProperties()
                   .FirstOrDefault(item => item.Name == "Id" && item.CanRead && item.PropertyType == typeof(Guid));

            if (propertyInfo == null) return;
            await Delete(applicationDbContext.Set<T>().FirstOrDefault(item => (Guid)propertyInfo.GetValue(item) == id));
        }

        virtual public async Task Save()
        {
            await applicationDbContext.SaveChangesAsync();
        }
    }
}
