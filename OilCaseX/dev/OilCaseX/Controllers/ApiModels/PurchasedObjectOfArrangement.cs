namespace OilCaseApi.Controllers.ApiModels
{
    public class PurchasedObjectOfArrangement
    {
        public string? Key { get; set; }
        public int CellX { get; set; }
        public int CellY { get; set; }
        public int SubCellX { get; set; }
        public int SubCellY { get; set; }
        public int GameStep { get; set; }
    }
    public class PurchasedObjectOfArrangementGet : PurchasedObjectOfArrangement
    {
        public int Id { get; set; }
    }
}