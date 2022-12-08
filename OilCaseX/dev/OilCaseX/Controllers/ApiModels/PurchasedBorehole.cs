﻿namespace OilCaseApi.Controllers.ApiModels;

public class PurchasedBorehole
{
    public string? Name { get; set; }
    public List<TrajectoryPoint> TrajectoryPoints { get; set; } = new();
}

public class PurchasedBoreholeProductionPost : PurchasedBorehole
{
    public int StatusId { get; set; }
}

public class PurchasedBoreholeExplorationPost : PurchasedBorehole
{
}

public class PurchasedBoreholeExplorationPatch
{
    public int Id { get; set; }
    public List<string>? LogNames { get; set; }
}

public class PurchasedBoreholeProductionPatch
{
    public int Id { get; set; }
    public int StatusId { get; set; }
}

public class PurchasedBoreholeGet : PurchasedBorehole
{
    public int Id { get; set; }
    public int GameStep { get; set; }
}

public class PurchasedBoreholeExplorationGet : PurchasedBoreholeGet
{
    public List<PurchasedLogName> LogNames { get; set; }
}

public class PurchasedBoreholeProductionGet : PurchasedBoreholeGet
{
    public int BoreholeStatusId { get; set; }
}