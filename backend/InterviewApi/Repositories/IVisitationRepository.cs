using InterviewApi.Models;

namespace InterviewApi.Repositories;

public interface IVisitationRepository
{
    Task<List<Visitation>> GetAllAsync();
    Task<Visitation?> GetByIdAsync(int id);
    Task<Visitation> AddAsync(Visitation visitation);
    Task<Visitation> UpdateAsync(Visitation visitation);
    Task<bool> DeleteAsync(int id);
    Task<List<Visitation>> GetByCustomerIdAsync(int customerId);
    Task<List<Visitation>> GetByHotelIdAsync(int hotelId);
    Task<List<Visitation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}