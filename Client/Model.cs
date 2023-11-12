using BlazorApp.Shared;
using System.Text.Json;

namespace BlazorApp.Client;

public class Model
{
    public List<Area>? areas {  get; set; }
    
    public static Model GetModel()
    {
        var file = "comments.json";
        // deserialize the json file
        var json = File.ReadAllText(file);
        return JsonSerializer.Deserialize<Model>(json);
    }
}

public class Area
{
    public required string name { get; set; }
    public List<Comment>? comments { get; set; }
}

public class Comment
{
    public int key { get; set; }
    public required string text { get; set; }
    public int rating { get; set; }
}