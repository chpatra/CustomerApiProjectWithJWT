﻿

namespace CutomerRepository
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll(); 
        Task<T> GetById(int id); 
        Task Add(T entity); 
        Task Update(T entity); 
        Task Delete(int id);
        Task AddCustomersAsync(IEnumerable<Customer> customers);
    }
}
