namespace OilCaseApi.Controllers.ApiModels
{
    public class BoreholeLog
    {
        public List<double> Positions { get; set; }
        public List<double> Values { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
