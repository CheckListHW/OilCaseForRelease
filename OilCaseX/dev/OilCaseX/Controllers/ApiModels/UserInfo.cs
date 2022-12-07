namespace OilCaseApi.Controllers.ApiModels;

public class TeamInfo
{
    public int? GameStep { get; set; }
    public LithologicalModelInfo LithologicalModelInfo { get; set; }
    public string Name { get; set; }
}

public class UserInfo
{
    public string? Role { get; set; }
    public string? Username { get; set; }
    public TeamInfo TeamInfo { get; set; }
}
