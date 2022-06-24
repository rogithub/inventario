using System;
using System.Linq;
namespace Ro.Inventario.Web.Models;

public static class CsvLineParser
{
    public static IEnumerable<string> ReadLines(this IFormFile file)
    {        
        if (file == null) yield break;        
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (reader.Peek() >= 0)
                yield return reader.ReadLine();
        }
    }
}
