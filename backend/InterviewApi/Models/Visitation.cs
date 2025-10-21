namespace InterviewApi.Models;

public class Visitation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int HotelId { get; set; }
    public DateTime VisitDate { get; set; }
}

public class VisitationData
{
    public List<Visitation> Visitations { get; set; } = new List<Visitation>();
}

public class VisitationDetail
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
}