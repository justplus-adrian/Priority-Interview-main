using InterviewApi.Models;
using System.Text.Json;

namespace InterviewApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers;
    private readonly object _lock = new object();

    public CustomerRepository()
    {
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "customers.json");
        _customers = LoadCustomersFromJson(dataPath);
    }

    private List<Customer> LoadCustomersFromJson(string dataPath)
    {
        try
        {
            if (!File.Exists(dataPath))
                return new List<Customer>();

            var json = File.ReadAllText(dataPath);
            var data = JsonSerializer.Deserialize<CustomerData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data?.Customers ?? new List<Customer>();
        }
        catch
        {
            return new List<Customer>();
        }
    }

    public Task<List<Customer>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_customers.ToList());
        }
    }

    public Task<Customer?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        }
    }

    public Task<Customer> AddAsync(Customer customer)
    {
        lock (_lock)
        {
            customer.Id = _customers.Any() ? _customers.Max(c => c.Id) + 1 : 1;
            customer.RegistrationDate = customer.RegistrationDate == default ? DateTime.UtcNow : customer.RegistrationDate;
            _customers.Add(customer);
            return Task.FromResult(customer);
        }
    }

    public Task<Customer> UpdateAsync(Customer customer)
    {
        lock (_lock)
        {
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.TotalPurchases = customer.TotalPurchases;
                return Task.FromResult(existingCustomer);
            }
            throw new KeyNotFoundException($"Customer with ID {customer.Id} not found");
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<List<Customer>> GetLoyalCustomersAsync(DateTime? date = null)
    {
        lock (_lock)
        {
            var targetDate = date ?? DateTime.UtcNow;
            var loyalCustomers = _customers
                .Where(c => c.RegistrationDate <= targetDate && c.TotalPurchases > 10)
                .ToList();
            return Task.FromResult(loyalCustomers);
        }
    }
} 