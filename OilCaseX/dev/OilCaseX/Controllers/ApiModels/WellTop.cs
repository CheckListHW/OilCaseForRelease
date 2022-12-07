namespace OilCaseApi.Controllers.ApiModels;

public class WellTop
{
    public int Id { get; set; }
    public int PurchasedBoreholeId { get; set; }
    public string Name { get; set; }
    public double Z { get; set; }
}