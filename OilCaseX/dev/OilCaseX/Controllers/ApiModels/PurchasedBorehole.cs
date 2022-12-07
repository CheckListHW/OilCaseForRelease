namespace OilCaseApi.Controllers.ApiModels;

public class PurchasedBorehole
{
    public string Name { get; set; }
    public int PurchasedObjectOfArrangementId { get; set; }
    public List<TrajectoryPoint> TrajectoryPoints { get; set; } = new();
}

public class PurchasedBoreholeProductionPost : PurchasedBorehole
{
    public int BoreholeStatusId { get; set; }
}

public class PurchasedBoreholeExplorationPost : PurchasedBorehole
{
    public List<string>? LogNames { get; set; }
}

public class PurchasedBoreholeExplorationPatch
{
    public int Id { get; set; }
    public List<string>? LogNames { get; set; }
}

public class PurchasedBoreholeProductionPatch
{
    public int Id { get; set; }
    public int? BoreholeStatusId { get; set; }
}

public class PurchasedBoreholeGet : PurchasedBorehole
{
    public int Id { get; set; }
    public int GameStep { get; set; }
}

public class PurchasedBoreholeExplorationGet : PurchasedBoreholeGet
{
    public List<LogName> LogNames { get; set; }
}

public class PurchasedBoreholeProductionGet : PurchasedBoreholeGet
{
    public int BoreholeStatusId { get; set; }
}