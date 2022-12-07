namespace OilCaseApi.Controllers.ApiModels;

public class LithologicalModelInfo
{
    public int MinX { get; set; }
    public int MinY { get; set; }
    public double? MinZ { get; set; }

    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public double? MaxZ { get; set; }

    public string Information { get; set; }
    public string Name { get; set; }
}