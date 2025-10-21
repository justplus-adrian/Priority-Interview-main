using InterviewApi.Models;
using System.Text.Json;

namespace InterviewApi.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly List<Hotel> _hotels;
    private readonly object _lock = new object();

    public HotelRepository()
    {
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "hotels.json");
        _hotels = LoadHotelsFromJson(dataPath);
    }

    private List<Hotel> LoadHotelsFromJson(string dataPath)
    {
        try
        {
            if (!File.Exists(dataPath))
                return new List<Hotel>();

            var json = File.ReadAllText(dataPath);
            var data = JsonSerializer.Deserialize<HotelData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data?.Hotels ?? new List<Hotel>();
        }
        catch
        {
            return new List<Hotel>();
        }
    }

    public Task<List<Hotel>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_hotels.ToList());
        }
    }

    public Task<Hotel?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Id == id);
            return Task.FromResult(hotel);
        }
    }

    public Task<Hotel> AddAsync(Hotel hotel)
    {
        lock (_lock)
        {
            hotel.Id = _hotels.Any() ? _hotels.Max(h => h.Id) + 1 : 1;
            _hotels.Add(hotel);
            return Task.FromResult(hotel);
        }
    }

    public Task<Hotel> UpdateAsync(Hotel hotel)
    {
        lock (_lock)
        {
            var existingHotel = _hotels.FirstOrDefault(h => h.Id == hotel.Id);
            if (existingHotel != null)
            {
                existingHotel.Name = hotel.Name;
                existingHotel.Address = hotel.Address;
                existingHotel.City = hotel.City;
                existingHotel.Country = hotel.Country;
                existingHotel.StarRating = hotel.StarRating;
                return Task.FromResult(existingHotel);
            }
            throw new KeyNotFoundException($"Hotel with ID {hotel.Id} not found");
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Id == id);
            if (hotel != null)
            {
                _hotels.Remove(hotel);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}