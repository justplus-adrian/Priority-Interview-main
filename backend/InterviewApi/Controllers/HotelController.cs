using Microsoft.AspNetCore.Mvc;
using InterviewApi.Models;
using InterviewApi.Repositories;

namespace InterviewApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IHotelRepository _hotelRepository;

    public HotelController(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    /// <summary>
    /// Get all hotels
    /// GET /api/hotel
    /// Response: List of all hotels
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Hotel>>> GetAllHotels()
    {
        var hotels = await _hotelRepository.GetAllAsync();
        return Ok(hotels);
    }

    /// <summary>
    /// Get a hotel by ID
    /// GET /api/hotel/{id}
    /// Response: Hotel details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(int id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        
        if (hotel == null)
        {
            return NotFound($"Hotel with ID {id} not found");
        }

        return Ok(hotel);
    }
}