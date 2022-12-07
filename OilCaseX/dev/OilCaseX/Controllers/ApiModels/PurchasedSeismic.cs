using DbModels = OilCaseApi.Models;

namespace OilCaseApi.Controllers.ApiModels;

public class PurchasedSeismic
{
    public int Id { get; set; }
    public int GameStep { get; set; }
    public int StartCellX { get; set; }
    public int StartCellY { get; set; }
    public int EndCellX { get; set; }
    public int EndCellY { get; set; }

    public static PurchasedSeismic FromPurchasedSeismic(DbModels.PurchasedSeismic ps)
    {
        return new()
        {
            Id = ps.Id,
            GameStep = ps.GameStep,
            StartCellX = ps.StartCellX,
            StartCellY = ps.StartCellY,
            EndCellX = ps.EndCellX,
            EndCellY = ps.EndCellY
        };
    }
}