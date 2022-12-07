namespace OilCaseApi.Controllers.ApiModels
{
    public class Seismic
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public List<double> Position { get; set; } = new();
        public Dictionary<string, List<double>> Values { get; set; } = new();
    }
}
