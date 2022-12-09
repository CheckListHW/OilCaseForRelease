namespace OilCaseApi.Controllers.ApiModels
{
    public class Borehole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double ZMin { get; set; }
        public double ZMax { get; set; }
        public List<BoreholeLog> BoreholeLog { get; set; } = new();
    }
}
