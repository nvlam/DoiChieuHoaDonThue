using System.IO;
using System.Text.Json;

public static class ExcelMappingLoader
{
    public static ExcelMappingRoot Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ExcelMappingRoot>(json)!;
    }
}
