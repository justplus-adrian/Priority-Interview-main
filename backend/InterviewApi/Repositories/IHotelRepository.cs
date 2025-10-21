using InterviewApi.Models;

namespace InterviewApi.Repositories;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllAsync();
    Task<Hotel?> GetByIdAsync(int id);
    Task<Hotel> AddAsync(Hotel hotel);
    Task<Hotel> UpdateAsync(Hotel hotel);
    Task<bool> DeleteAsync(int id);
}