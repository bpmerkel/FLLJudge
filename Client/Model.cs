using BlazorApp.Shared;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp.Client;

public class Model
{
    public List<Area>? areas {  get; set; }
    
    public static Model GetModel()
    {
        var file = "comments.json";
        // deserialize the json file
        var json = File.ReadAllText(file);
        var model = JsonSerializer.Deserialize<Model>(json);
        foreach(var area in model.areas)
        {
            if (area == null) continue;
            area.chips = area.comments
                .SelectMany(c =>
                {
                    return c.text.Split(" \t;.:,".ToCharArray())
                        .Where(w => w.Length > 3)
                        .GroupBy(w => w)
                        .Where(g => g.Count() < 7)
                        .SelectMany(g => g);
                })
                .ToList();
        }
        return model;
    }
}

public class Area
{
    public required string name { get; set; }
    public List<Comment>? comments { get; set; }
    [JsonIgnore]
    public List<string>? chips { get; set; }
}

public class Comment
{
    public int key { get; set; }
    public required string text { get; set; }
    public int rating { get; set; }
}