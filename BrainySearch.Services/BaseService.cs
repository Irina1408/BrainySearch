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
        List<T> GetAll();
        T GetById(Guid id);
        
        void Add(T item);
        void Delete(T item);
        void DeleteById(Guid id);
        void Save();
    }

    public class BaseService<T> : IBaseService<T>
        where T : class, new()
    {
        protected readonly ApplicationDbContext applicationDbContext;

        public BaseService(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        virtual public List<T> GetAll()
        {
            return applicationDbContext.Set<T>().ToList();
        }

        virtual public T GetById(Guid id)
        {
            var propertyInfo = typeof(T).GetProperties()
                .FirstOrDefault(item => item.Name == "Id" && item.CanRead && item.PropertyType == typeof(Guid));

            if (propertyInfo == null) return null;
            else return  applicationDbContext.Set<T>().FirstOrDefault(item => (Guid)propertyInfo.GetValue(item) == id);
        }

        virtual public void Add(T item)
        {
            applicationDbContext.Set<T>().Add(item);
        }

        virtual public void Delete(T item)
        {
            applicationDbContext.Set<T>().Remove(item);
        }

        virtual public void DeleteById(Guid id)
        {
            var propertyInfo = typeof(T).GetProperties()
                   .FirstOrDefault(item => item.Name == "Id" && item.CanRead && item.PropertyType == typeof(Guid));

            if (propertyInfo == null) return;
            Delete(applicationDbContext.Set<T>().FirstOrDefault(item => (Guid)propertyInfo.GetValue(item) == id));
        }

        virtual public void Save()
        {
            applicationDbContext.SaveChanges();
        }
    }
}
