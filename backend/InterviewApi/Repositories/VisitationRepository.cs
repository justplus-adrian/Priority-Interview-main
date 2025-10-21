using InterviewApi.Models;
using System.Text.Json;

namespace InterviewApi.Repositories;

public class VisitationRepository : IVisitationRepository
{
    private readonly List<Visitation> _visitations;
    private readonly object _lock = new object();

    public VisitationRepository()
    {
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "visitations.json");
        _visitations = LoadVisitationsFromJson(dataPath);
    }

    private List<Visitation> LoadVisitationsFromJson(string dataPath)
    {
        try
        {
            if (!File.Exists(dataPath))
                return new List<Visitation>();

            var json = File.ReadAllText(dataPath);
            var data = JsonSerializer.Deserialize<VisitationData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data?.Visitations ?? new List<Visitation>();
        }
        catch
        {
            return new List<Visitation>();
        }
    }

    public Task<List<Visitation>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_visitations.ToList());
        }
    }

    public Task<Visitation?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var visitation = _visitations.FirstOrDefault(v => v.Id == id);
            return Task.FromResult(visitation);
        }
    }

    public Task<Visitation> AddAsync(Visitation visitation)
    {
        lock (_lock)
        {
            visitation.Id = _visitations.Any() ? _visitations.Max(v => v.Id) + 1 : 1;
            _visitations.Add(visitation);
            return Task.FromResult(visitation);
        }
    }

    public Task<Visitation> UpdateAsync(Visitation visitation)
    {
        lock (_lock)
        {
            var existingVisitation = _visitations.FirstOrDefault(v => v.Id == visitation.Id);
            if (existingVisitation != null)
            {
                existingVisitation.CustomerId = visitation.CustomerId;
                existingVisitation.HotelId = visitation.HotelId;
                existingVisitation.VisitDate = visitation.VisitDate;
                return Task.FromResult(existingVisitation);
            }
            throw new KeyNotFoundException($"Visitation with ID {visitation.Id} not found");
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            var visitation = _visitations.FirstOrDefault(v => v.Id == id);
            if (visitation != null)
            {
                _visitations.Remove(visitation);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<List<Visitation>> GetByCustomerIdAsync(int customerId)
    {
        lock (_lock)
        {
            var visitations = _visitations.Where(v => v.CustomerId == customerId).ToList();
            return Task.FromResult(visitations);
        }
    }

    public Task<List<Visitation>> GetByHotelIdAsync(int hotelId)
    {
        lock (_lock)
        {
            var visitations = _visitations.Where(v => v.HotelId == hotelId).ToList();
            return Task.FromResult(visitations);
        }
    }

    public Task<List<Visitation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        lock (_lock)
        {
            var visitations = _visitations
                .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate)
                .ToList();
            return Task.FromResult(visitations);
        }
    }
}