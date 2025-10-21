using InterviewApi.Models;

namespace InterviewApi.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(int id);
    Task<List<Customer>> GetLoyalCustomersAsync(DateTime? date = null);
}