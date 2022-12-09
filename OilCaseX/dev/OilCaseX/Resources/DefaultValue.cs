using System.Diagnostics;
using Newtonsoft.Json;
using OilCaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace OilCaseApi.resources;

public struct RequiredBoreholeStatus
{
    public const string wt1 = "wt1";
    public const string wt2 = "wt2";
    public const string wt3 = "wt3";
    public const string wt4 = "wt4";
    public const string wt5 = "wt5";
    public const string wt6 = "wt6";
}

public struct RequiredColumnNames
{
    public const string Facies = "facies";
    public const string X = "x";
    public const string Y = "y";
    public const string Z = "z";
}

public class InitObjectsOfArrangement
{
    public IEnumerable<ObjectOfArrangement> ObjectsOfArrangement { get; set; }
}

public class DefaultValue
{
    public static double? GetValueByName(string name) =>
        Convert.ToDouble(_Get(name, "Resources/DefaultValue.json"));

    private static string? _Get(string name, string path)
    {
        TextReader reader = new StreamReader(path);
        var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
        return json != null && json.ContainsKey(name) ? json[name] : null;
    }

    public static async Task<IActionResult?> InitBdStartValuesAsync(ApplicationContext context)
    {
        TextReader reader = new StreamReader("Resources/ObjectsOfArrangement.json");
        var json = JsonConvert.DeserializeObject<InitObjectsOfArrangement>(reader.ReadToEnd());
        foreach (var item in json.ObjectsOfArrangement.ToArray())
            context.ObjectsOfArrangement.Add(item);

        LithologicalModel lithologicalModel = new()
        {
            Name = "FirstModel"
        };

        Team team = new Team()
        {
            Name = "FirstTeam",
            LithologicalModel = lithologicalModel
        };

        context.Team.Add(team);

        foreach (var field in typeof(RequiredBoreholeStatus).GetFields())
            context.BoreholeStatus.Add(new BoreholeStatus()
            {
                Key = field.Name,
            });


        await context.SaveChangesAsync();
        context.LithologicalModel.First()?.LoadSeismicFromCsv("Files/Seismic.csv");
        context.LithologicalModel.First()?.LoadBoreholeLogsFromCsv("Files/PropsShort.csv");
        return null;
    }
}