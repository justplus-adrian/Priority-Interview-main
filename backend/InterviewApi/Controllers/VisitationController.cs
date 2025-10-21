using Microsoft.AspNetCore.Mvc;
using InterviewApi.Models;
using InterviewApi.Repositories;

namespace InterviewApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitationController : ControllerBase
{
    private readonly IVisitationRepository _visitationRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IHotelRepository _hotelRepository;

    public VisitationController(
        IVisitationRepository visitationRepository,
        ICustomerRepository customerRepository,
        IHotelRepository hotelRepository)
    {
        _visitationRepository = visitationRepository;
        _customerRepository = customerRepository;
        _hotelRepository = hotelRepository;
    }

    /// <summary>
    /// Get all visitations with customer and hotel details
    /// GET /api/visitation
    /// Response: List of all visitations with customer name and hotel name
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<VisitationDetail>>> GetAllVisitations()
    {
        var visitations = await _visitationRepository.GetAllAsync();
        var customers = await _customerRepository.GetAllAsync();
        var hotels = await _hotelRepository.GetAllAsync();

        var visitationDetails = visitations.Select(v => new VisitationDetail
        {
            Id = v.Id,
            CustomerId = v.CustomerId,
            CustomerName = customers.FirstOrDefault(c => c.Id == v.CustomerId)?.Name ?? "Unknown Customer",
            HotelId = v.HotelId,
            HotelName = hotels.FirstOrDefault(h => h.Id == v.HotelId)?.Name ?? "Unknown Hotel",
            VisitDate = v.VisitDate
        }).ToList();

        return Ok(visitationDetails);
    }

    /// <summary>
    /// Get a visitation by ID with customer and hotel details
    /// GET /api/visitation/{id}
    /// Response: Visitation details with customer name and hotel name
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VisitationDetail>> GetVisitation(int id)
    {
        var visitation = await _visitationRepository.GetByIdAsync(id);
        
        if (visitation == null)
        {
            return NotFound($"Visitation with ID {id} not found");
        }

        var customer = await _customerRepository.GetByIdAsync(visitation.CustomerId);
        var hotel = await _hotelRepository.GetByIdAsync(visitation.HotelId);

        var visitationDetail = new VisitationDetail
        {
            Id = visitation.Id,
            CustomerId = visitation.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            HotelId = visitation.HotelId,
            HotelName = hotel?.Name ?? "Unknown Hotel",
            VisitDate = visitation.VisitDate
        };

        return Ok(visitationDetail);
    }

    /// <summary>
    /// Get visitations by customer ID with hotel details
    /// GET /api/visitation/customer/{customerId}
    /// Response: List of visitations for a specific customer with hotel names
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<VisitationDetail>>> GetVisitationsByCustomer(int customerId)
    {
        var visitations = await _visitationRepository.GetByCustomerIdAsync(customerId);
        var customer = await _customerRepository.GetByIdAsync(customerId);
        var hotels = await _hotelRepository.GetAllAsync();

        if (customer == null)
        {
            return NotFound($"Customer with ID {customerId} not found");
        }

        var visitationDetails = visitations.Select(v => new VisitationDetail
        {
            Id = v.Id,
            CustomerId = v.CustomerId,
            CustomerName = customer.Name,
            HotelId = v.HotelId,
            HotelName = hotels.FirstOrDefault(h => h.Id == v.HotelId)?.Name ?? "Unknown Hotel",
            VisitDate = v.VisitDate
        }).ToList();

        return Ok(visitationDetails);
    }

    /// <summary>
    /// Get visitations by hotel ID with customer details
    /// GET /api/visitation/hotel/{hotelId}
    /// Response: List of visitations for a specific hotel with customer names
    /// </summary>
    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<List<VisitationDetail>>> GetVisitationsByHotel(int hotelId)
    {
        var visitations = await _visitationRepository.GetByHotelIdAsync(hotelId);
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);
        var customers = await _customerRepository.GetAllAsync();

        if (hotel == null)
        {
            return NotFound($"Hotel with ID {hotelId} not found");
        }

        var visitationDetails = visitations.Select(v => new VisitationDetail
        {
            Id = v.Id,
            CustomerId = v.CustomerId,
            CustomerName = customers.FirstOrDefault(c => c.Id == v.CustomerId)?.Name ?? "Unknown Customer",
            HotelId = v.HotelId,
            HotelName = hotel.Name,
            VisitDate = v.VisitDate
        }).ToList();

        return Ok(visitationDetails);
    }

    /// <summary>
    /// Get visitations by date range with customer and hotel details
    /// GET /api/visitation/daterange?startDate=2024-01-01&endDate=2024-12-31
    /// Response: List of visitations within date range with customer and hotel names
    /// </summary>
    [HttpGet("daterange")]
    public async Task<ActionResult<List<VisitationDetail>>> GetVisitationsByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date cannot be greater than end date");
        }

        var visitations = await _visitationRepository.GetByDateRangeAsync(startDate, endDate);
        var customers = await _customerRepository.GetAllAsync();
        var hotels = await _hotelRepository.GetAllAsync();

        var visitationDetails = visitations.Select(v => new VisitationDetail
        {
            Id = v.Id,
            CustomerId = v.CustomerId,
            CustomerName = customers.FirstOrDefault(c => c.Id == v.CustomerId)?.Name ?? "Unknown Customer",
            HotelId = v.HotelId,
            HotelName = hotels.FirstOrDefault(h => h.Id == v.HotelId)?.Name ?? "Unknown Hotel",
            VisitDate = v.VisitDate
        }).ToList();

        return Ok(visitationDetails);
    }

    /// <summary>
    /// Add a new visitation
    /// POST /api/visitation
    /// Request body: { "customerId": 1, "hotelId": 1, "visitDate": "2024-01-15" }
    /// Response: Created visitation with customer and hotel details
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VisitationDetail>> AddVisitation([FromBody] Visitation visitation)
    {
        // Validate that customer and hotel exist
        var customer = await _customerRepository.GetByIdAsync(visitation.CustomerId);
        var hotel = await _hotelRepository.GetByIdAsync(visitation.HotelId);

        if (customer == null)
        {
            return BadRequest($"Customer with ID {visitation.CustomerId} not found");
        }

        if (hotel == null)
        {
            return BadRequest($"Hotel with ID {visitation.HotelId} not found");
        }

        // Set default visit date if not provided
        if (visitation.VisitDate == default)
        {
            visitation.VisitDate = DateTime.UtcNow;
        }

        var createdVisitation = await _visitationRepository.AddAsync(visitation);

        var visitationDetail = new VisitationDetail
        {
            Id = createdVisitation.Id,
            CustomerId = createdVisitation.CustomerId,
            CustomerName = customer.Name,
            HotelId = createdVisitation.HotelId,
            HotelName = hotel.Name,
            VisitDate = createdVisitation.VisitDate
        };

        return CreatedAtAction(nameof(GetVisitation), new { id = createdVisitation.Id }, visitationDetail);
    }
}