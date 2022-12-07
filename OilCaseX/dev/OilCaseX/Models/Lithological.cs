using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;

namespace OilCaseApi.Models;

public class LithologicalModel
{
    public static readonly List<string> NotGisColumns = new() { "x", "y", "z" };
    [Key] public int Id { get; set; }
    public string Name { get; set; }

    public string GetInfo()
    {
        var context = new ApplicationContext();
        int seismicCount = context.Seismic.Count(s => s.LithologicalModelId == Id);

        var lithologicalCells = context.Cells.Where(c => c.LithologicalModelId == Id);
        var lithologicalCellIds = lithologicalCells.Select(c => c.Id).ToList();

        if (!lithologicalCellIds.Any())
            return $"Logs Ceil count: 0, Seismic Count: {seismicCount}";


        var logs = lithologicalCellIds.Select(lci =>
            context.BoreholeLogs.Where(bl => bl.CellId == lci).Select(bl => bl.Z));
        if (!logs.Any())
            return $"Logs Ceil count: 0, Seismic Count: {seismicCount}";

        MinX = lithologicalCells.Min(c => c.X);
        MaxX = lithologicalCells.Max(c => c.X);
        MinY = lithologicalCells.Min(c => c.Y);
        MaxY = lithologicalCells.Max(c => c.Y);
        MinZ = logs.Min(l => l.Min());
        MaxZ = logs.Max(l => l.Max());

        var logNames = context.LogNames.Where(ln => ln.LithologicalModel == this).Select(ln => ln.Name.ToUpper());
        CountCeil = lithologicalCellIds.Sum(id => context.BoreholeLogs.Count(bl => bl.CellId == id));
        return
            $"X({MinX}:{MaxX}), Y({MinY}:{MaxY}), Z({MinZ}:{MaxZ}),Logs ceil count: {CountCeil}. Seismic ceil Count: {seismicCount}\n" +
            $"Gis ({logNames.Count()}): " + string.Join(", ", logNames);
    }

    public string? _Information { get; set; }
    public double MetricBoreholeLogTableChange { get; set; }

    public int MinX { get; set; }
    public int MinY { get; set; }
    public double? MinZ { get; set; }

    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public double? MaxZ { get; set; }

    public int CountCeil { get; set; }


    [NotMapped]
    public string? Information
    {
        get
        {
            var context = new ApplicationContext();
            var cellsId = context.Cells.Where(c => c.LithologicalModelId == Id).Select(c => c.Id).ToArray();
            var seismics = context.Seismic.Where(s => s.LithologicalModelId == Id);
            double metric = (new Random().NextDouble());
            if (cellsId.Length > 0)
            {
                var cellIndex = new Random().Next(0, cellsId.Count());
                metric = context.BoreholeLogs.Where(bl => bl.CellId == cellsId[cellIndex])
                    .Sum(bl => bl.Z) + seismics.Sum(s => s.Z);
            }

            if (_Information == null || MetricBoreholeLogTableChange != metric)
            {
                MetricBoreholeLogTableChange = metric;
                _Information = GetInfo();
                context.LithologicalModel.Update(this);
                context.SaveChanges();
            }

            return _Information;
        }
        set => _Information = value;
    }


    private static string? CsvValidation(string path)
    {
        string? message;
        using (var fileStream = new FileStream(path, FileMode.Open))
        {
            using (var parser = new TextFieldParser(fileStream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                message = _CsvValidation(parser);
            }
        }

        return message;
    }

    private static string? _CsvValidation(TextFieldParser parser)
    {
        try
        {
            const string messageInputColumnName = "Необходимо удалить колонну без имени номер {0}";
            const string messageNotExistColumnName = "Отсутствует колонна {0}. \n" +
                                                     "Для корректной загрузки необходимы следующие колонны {1}\n" +
                                                     "Имеются следующие колонны {2}\n";
            const string messageNotExistGis = "Отсутствуют Кривые";
            const string messageNotConvertibleValue =
                "В строке {0} колонне {1} значение {2} нельзя интерпретировать.\n";

            var gisNames = parser.ReadFields()!.Select(x => x.ToLower()).ToList();

            for (var i = 0; i < gisNames.Count; i++)
            {
                if (string.IsNullOrEmpty(gisNames[i]))
                    return string.Format(messageInputColumnName, i);
            }

            // проверка на наличие координат.
            foreach (var name in NotGisColumns)
                if (!gisNames.Contains(name.ToLower()))
                    return string.Format(messageNotExistColumnName, name, string.Join(", ", NotGisColumns),
                        string.Join(", ", gisNames));

            if (gisNames.Count < 4)
                return messageNotExistGis;

            double d;
            int row = 0;
            while (!parser.EndOfData)
            {
                row++;
                int column = 0;
                string[]? fields = parser.ReadFields();
                foreach (var field in fields!)
                {
                    if (!double.TryParse(field.Replace(".", ","), out d))
                        return string.Format(messageNotConvertibleValue, row, gisNames[column], field);
                    column++;
                }

                if (column != gisNames.Count)
                    return $"В строке {row} обнаружено {column} значений, а должно быть {gisNames.Count}.\n" +
                           $"Имена колонн: {string.Join("; ", gisNames)}\n" +
                           $"Строка {row}: {string.Join("; ", fields)}\n" +
                           $"{parser.EndOfData}";
            }

            return null;
        }
        catch (Exception e)
        {
            return $"Не удалось считать файл:\n\n {e}";
        }
    }

    public string? LoadBoreholeLogsFromCsv(string path)
    {
        var timer = new Stopwatch();
        timer.Start();
        var context = new ApplicationContext();
        string? validationMessage = CsvValidation(path);
        if (validationMessage != null)
            return validationMessage;

        var file = File.OpenText(path);

        var names = file.ReadLine()?.Split(';').Select(x => x.ToLower()).ToList();
        var existingLogNames = context.LogNames.Where(ln => ln.LithologicalModelId == Id).ToArray();
        var logNames = names
            .Select(n => existingLogNames.FirstOrDefault(ln => ln.Name == n,
                new LogName()
                {
                    LithologicalModelId = Id,
                    Name = n,
                    Description = n,
                    Group = ""
                })).ToArray();

        context.LogNames.AddRange(logNames.Where(ln => ln.Id == 0 & !NotGisColumns.Contains(ln.Name)));
        context.SaveChanges();

        int xIndex = names.IndexOf(NotGisColumns[0]),
            yIndex = names.IndexOf(NotGisColumns[1]),
            zIndex = names.IndexOf(NotGisColumns[2]),
            countCeil = 0;

        var copy = BoreholeLog.GetBulkCopy(ApplicationContext.ConnectionString);
        var dt = new DataTable();
        foreach (var s in new[] { "Id", "LogNameId", "CellId", "Z", "Value" })
            dt.Columns.Add(s);

        var cells = context.Cells.Where(c => c.LithologicalModelId == Id)
            .ToDictionary(cell => $"{cell.X}-{cell.Y}-{Id}", cell => cell);

        while (file.ReadLine()?.Replace('.', ',').Split(';') is { } tableRow)
        {
            int x = Convert.ToInt32(tableRow?[xIndex]),
                y = Convert.ToInt32(tableRow?[yIndex]);
            double z = Convert.ToDouble(tableRow?[zIndex]);

            string cellKey = $"{x}-{y}-{Id}";
            if (!cells.ContainsKey(cellKey))
            {
                cells[cellKey] = new Cell()
                {
                    LithologicalModelId = Id,
                    X = x,
                    Y = y
                };
                context.Cells.Add(cells[cellKey]);
                context.SaveChanges();
            }

            for (var i = 0; i < tableRow!.Length; i++)
                if (!NotGisColumns.Contains(logNames[i].Name))
                    dt.Rows.Add(null, logNames[i].Id, cells[cellKey].Id, z, tableRow[i]);

            if (dt.Rows is { Count: < 10000 }) continue;
            Debug.WriteLine((countCeil += dt.Rows.Count));
            copy.WriteToServer(dt);
            dt.Rows.Clear();
        }

        file.Close();
        copy.WriteToServer(dt);

        timer.Stop();
        Debug.WriteLine($"Finish time:{timer.Elapsed:m\\:ss\\.fff}");

        context.LithologicalModel.Update(this);
        context.SaveChanges();

        return null;
    }
    public void DeleteDuplicateRows()
    {
        var timer = new Stopwatch();
        timer.Start();

        var context = new ApplicationContext();
        var tasks = new List<Task>();
        foreach (var cellId in context.Cells.Where(c => c.LithologicalModelId == Id).Select(c => c.Id).ToArray())
        {
            tasks.Add(new Task(() =>
            {
                var taskContext = new ApplicationContext();
                var boreholeLogGroups = from b in taskContext.BoreholeLogs.Where(b => b.CellId == cellId).ToList()
                    group b by new { b.Z, b.LogNameId };

                foreach (var boreholeLogGroup in boreholeLogGroups.ToList())
                    taskContext.BoreholeLogs.RemoveRange(boreholeLogGroup.Where(s => s.Id != boreholeLogGroup.Max(s => s.Id)));
                taskContext.SaveChanges();
            }));
            tasks.Last().Start();
        }

        var seismicGroups = from s in context.Seismic.Where(s => s.LithologicalModelId == Id).ToList()
            group s by new { s.LayerName, s.X, s.Y };

        foreach (var seismicGroup in seismicGroups.ToList())
            context.Seismic.RemoveRange(seismicGroup.Where(s => s.Id != seismicGroup.Max(s => s.Id)));

        Task.WaitAll(tasks.ToArray());
        context.SaveChanges();
       
        timer.Stop();
        Debug.WriteLine($"Finish time:{timer.Elapsed:m\\:ss\\.fff}");
    }

    public void DeleteAll()
    {
        var context = new ApplicationContext();
        context.LogNames.RemoveRange(context.LogNames.Where(c => c.LithologicalModelId == Id));
        context.Cells.RemoveRange(context.Cells.Where(c => c.LithologicalModelId == Id));
        context.SaveChanges();
    }

    public List<Team>? Teams { get; set; }

    public string? LoadSeismicFromCsv(string path)
    {
        var context = new ApplicationContext();
        var file = File.OpenText(path);
        var names = file.ReadLine()?.Split(';').Select(x => x.ToLower()).ToList();
        var cells = context.Cells.Where(c => c.LithologicalModel == this).ToArray();
        while (file.ReadLine() is { } line)
        {
            var values = line.Split(';').Select(x => x.ToLower()).ToList();
            int x = Convert.ToInt16(values[0]);
            int y = Convert.ToInt16(values[1]);

            context.Seismic.AddRange(new[]
            {
                new Seismic()
                {
                    X = x,
                    Y = y,
                    LithologicalModelId = Id,
                    LayerName = names[2],
                    Z = Convert.ToDouble(values[2])
                },
                new Seismic()
                {
                    X = x,
                    Y = y,
                    LithologicalModelId = Id,
                    LayerName = names[3],
                    Z = Convert.ToDouble(values[3])
                }
            });
        }

        context.SaveChanges();
        return null;
    }

    public List<Cell> Cells { get; set; }
    public List<LogName> LogNames { get; set; }
}

public class Cell
{
    [Key] public int Id { get; set; }
    public LithologicalModel LithologicalModel { get; set; }
    public int LithologicalModelId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public List<BoreholeLog>? BoreholeLogs { get; set; }
}

public class LogName
{
    [Key] public int Id { get; set; }
    public int LithologicalModelId { get; set; }
    public LithologicalModel LithologicalModel { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Group { get; set; }
    public double CostDay { get; set; } = 5;
    public double CostMoney { get; set; } = 1000;
}

[Index("CellId")]
public class BoreholeLog
{
    public int Id { get; set; }
    public Cell Cell { get; set; }
    public int CellId { get; set; }
    public double Z { get; set; }

    public static SqlBulkCopy GetBulkCopy(string connectionString)
    {
        var copy = new SqlBulkCopy(connectionString);
        copy.DestinationTableName = "dbo.BoreholeLogs";
        copy.ColumnMappings.Add(nameof(Id), "Id");
        copy.ColumnMappings.Add(nameof(LogNameId), "LogNameId");
        copy.ColumnMappings.Add(nameof(CellId), "CellId");
        copy.ColumnMappings.Add(nameof(Z), "Z");
        copy.ColumnMappings.Add(nameof(Value), "Value");
        return copy;
    }

    public int LogNameId { get; set; }
    public LogName LogName { get; set; }
    public double Value { get; set; }
}

[Index("LithologicalModelId")]
public class Seismic
{
    public int Id { get; set; }
    public int LithologicalModelId { get; set; }
    public LithologicalModel LithologicalModel { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public string? LayerName { get; set; }
}