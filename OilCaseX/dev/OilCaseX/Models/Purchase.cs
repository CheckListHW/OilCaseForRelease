using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace OilCaseApi.Models;

public interface IPurchased
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public int GameStep { get; set; }
}

public class BoreholeStatus {
    [Key] public int Id { get; set; }
    public string Name { get; set; }
}


public class BoreholeStatusHistory
{
    [Key] public int Id { get; set; }
    public int GameStep { get; set; }
    public int PurchasedBoreholeId { get; set; }
    public PurchasedBorehole PurchasedBorehole { get; set; }
    public int BoreholeStatusId { get; set; }
    public BoreholeStatus BoreholeStatus { get; set; }

}

public class TrajectoryPoint
{
    [Key] public int Id { get; set; }
    public int PurchasedBoreholeId { get; set; }
    public PurchasedBorehole? PurchasedBorehole { get; set; }
    public int CellX { get; set; }
    public int CellY { get; set; }
    public double CellZ { get; set; }
}

public class ObjectOfArrangement
{
    [Key] public int Id { get; set; }
    public string Key { get; set; }
    public string? Description { get; set; }
    public double CostMoney { get; set; } = 0;
    public double CostDay { get; set; } = 0;
    public string? ModelGLTfName { get; set; }
    public int? Part { get; set; }
    public string? Alias { get; set; }
    public string? Color { get; set; }
}

public class PurchasedBorehole : IPurchased
{
    [Key] public int Id { get; set; }
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public int GameStep { get; set; }

    // soKust Кустовая площадка
    [Required] public int PurchasedObjectOfArrangementId { get; set; }
    public PurchasedObjectOfArrangement PurchasedObjectOfArrangement { get; set; }
    public string Name { get; set; }
    public IEnumerable<TrajectoryPoint> TrajectoryPoints { get; set; }
    public List<BoreholeStatusHistory> BoreholeStatusHistories { get; set; }

    public IEnumerable<WellTop> WellTops { get; set; }
    public List<PurchasedLogName> PurchasedLogNames { get; set; }

    // if False is Exploration
    public bool IsProduction { get; set; }
}


public class WellTop
{
    [Key] public int Id { get; set; }
    public int PurchasedBoreholeId { get; set; }
    public PurchasedBorehole PurchasedBorehole { get; set; }
    public double Z { get; set; }
    public string? Name { get; set; }
}

public class PurchasedSeismic : IPurchased
{
    [Key] public int Id { get; set; }

    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public int GameStep { get; set; }

    public int StartCellX { get; set; }
    public int StartCellY { get; set; }
    public int EndCellX { get; set; }
    public int EndCellY { get; set; }

    public static bool operator ==(PurchasedSeismic? a, PurchasedSeismic? b)
        => (a?.StartCellX == b?.StartCellX)
           & (a?.StartCellY == b?.StartCellY)
           & (a?.EndCellX == b?.EndCellX)
           & (a?.EndCellY == b?.EndCellY)
           & (a?.TeamId == b?.TeamId || a?.Team?.Id == b?.Team?.Id);

    public static bool operator !=(PurchasedSeismic? a, PurchasedSeismic? b)
        => !(a == b);
}

public class PurchasedObjectOfArrangement : IPurchased
{
    [Key] public int Id { get; set; }

    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public int GameStep { get; set; }
    public int ObjectOfArrangementId { get; set; }
    public ObjectOfArrangement? ObjectOfArrangement { get; set; }

    public int CellX { get; set; }
    public int CellY { get; set; }
    public int SubCellX { get; set; }
    public int SubCellY { get; set; }

    public static bool operator ==(PurchasedObjectOfArrangement? a, PurchasedObjectOfArrangement? b)
        => (a?.CellX == b?.CellX) & (a?.CellY == b?.CellY)
                                  & (a?.SubCellX == b?.SubCellX) & (a?.SubCellY == b?.SubCellY)
                                  & (a?.TeamId == b?.TeamId || a?.Team?.Id == b?.Team?.Id);
    public static bool operator !=(PurchasedObjectOfArrangement? a, PurchasedObjectOfArrangement? b)
        => !(a == b);
}

public class PurchasedLogName 
{
    [Key] public int Id { get; set; }
    public int PurchasedBoreholeId { get; set; }
    public PurchasedBorehole PurchasedBorehole { get; set; }
    public int LogNameId { get; set; }
    public LogName LogName { get; set; }
}

public class PurchasedMap : IPurchased
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public int GameStep { get; set; }
    public string Name { get; set; }
}