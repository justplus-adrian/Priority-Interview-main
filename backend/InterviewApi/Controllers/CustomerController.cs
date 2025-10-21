using Microsoft.AspNetCore.Mvc;
using InterviewApi.Models;
using InterviewApi.Repositories;

namespace InterviewApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Welcome endpoint - returns a welcome message
    /// </summary>
    [HttpGet("welcome")]
    public ActionResult<object> Welcome()
    {
        return Ok(new
        {
            message = "Welcome to Priority Customer Management API!",
            version = "1.0.0",
            dataSource = "In-memory data loaded from JSON file: Data/customers.json",
            endpoints = new[]
            {
                "GET /api/customer/welcome - This endpoint",
                "POST /api/customer - Add new customer",
                "GET /api/customer/{id} - Get a customer by ID",
                "GET /api/customer/loyal - Find loyal customers at date",
                "POST /api/customer/register - Register a customer at date"
            },
            note = "Uses CustomerRepository for in-memory data management"
        });
    }

    /// <summary>
    /// Add a new customer
    /// POST /api/customer
    /// Request body: { "name": "John Doe", "email": "john@example.com" }
    /// Response: Created customer with ID
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Customer>> AddCustomer([FromBody] Customer customer)
    {
        // Your implementation here:
        // 1. Validate the customer data (name, email required)
        // 2. Read existing customers from JSON: var customers = ReadCustomersFromJson();
        // 3. Generate new ID: customer.Id = customers.Max(c => c.Id) + 1;
        // 4. Set registration date: customer.RegistrationDate = DateTime.Now;
        // 5. Add to list: customers.Add(customer);
        // 6. Save to JSON: WriteCustomersToJson(customers);
        // 7. Return 201 Created status

        // Validate the customer data (name, email required)
        if (string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Email))
        {
            return BadRequest("Name and Email are required");
        }

        // Set default values
        customer.RegistrationDate = DateTime.UtcNow;
        customer.TotalPurchases = 0;

        // Add customer using repository
        var createdCustomer = await _customerRepository.AddAsync(customer);

        return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomer);
    }

    /// <summary>
    /// Get a customer by ID
    /// GET /api/customer/{id}
    /// Response: Customer details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        // Your implementation here:
        // 1. Read customers from JSON: var customers = ReadCustomersFromJson();
        // 2. Find customer by ID: var customer = customers.FirstOrDefault(c => c.Id == id);
        // 3. Return 404 if not found: if (customer == null) return NotFound();
        // 4. Return customer if found: return Ok(customer);

        var customer = await _customerRepository.GetByIdAsync(id);
        
        if (customer == null)
        {
            return NotFound($"Customer with ID {id} not found");
        }

        return Ok(customer);
    }

    /// <summary>
    /// Find loyal customers at a specific date
    /// GET /api/customer/loyal?date=2024-01-01
    /// Query parameter: date (optional, defaults to today)
    /// Response: List of loyal customers (customers with TotalPurchases > 10)
    /// </summary>
    [HttpGet("loyal")]
    public async Task<ActionResult<List<Customer>>> GetLoyalCustomers([FromQuery] DateTime? date)
    {
        // TODO: Implement this endpoint
        // Find loyal customers at a specific date
        // GET /api/customer/loyal?date=2024-01-01
        // Query parameter: date (optional, defaults to today)
        // Response: List of loyal customers (e.g., customers with TotalPurchases > 10)

        var loyalCustomers = await _customerRepository.GetLoyalCustomersAsync(date);
        return Ok(loyalCustomers);
    }

    /// <summary>
    /// Register a customer at a specific date
    /// POST /api/customer/register
    /// Request body: { "name": "Jane Doe", "email": "jane@example.com", "registrationDate": "2024-01-01" }
    /// Response: Registered customer
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<Customer>> RegisterCustomer([FromBody] Customer customer)
    {
        // Your implementation here:
        // 1. Validate customer data (name, email required)
        // 2. Read existing customers from JSON: var customers = ReadCustomersFromJson();
        // 3. Generate new ID: customer.Id = customers.Max(c => c.Id) + 1;
        // 4. Use RegistrationDate from request (or default to DateTime.Now if not provided)
        // 5. Set TotalPurchases to 0 for new customer: customer.TotalPurchases = 0;
        // 6. Add to list: customers.Add(customer);
        // 7. Save to JSON: WriteCustomersToJson(customers);
        // 8. Return 201 Created status

        // Validate customer data (name, email required)
        if (string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Email))
        {
            return BadRequest("Name and Email are required");
        }

        // Use RegistrationDate from request (or default to DateTime.UtcNow if not provided)
        if (customer.RegistrationDate == default)
        {
            customer.RegistrationDate = DateTime.UtcNow;
        }

        // Set TotalPurchases to 0 for new customer
        customer.TotalPurchases = 0;

        // Add customer using repository
        var registeredCustomer = await _customerRepository.AddAsync(customer);

        return CreatedAtAction(nameof(GetCustomer), new { id = registeredCustomer.Id }, registeredCustomer);
    }


}

