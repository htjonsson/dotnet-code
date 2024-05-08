using Newtonsoft.Json;

namespace Graph.Models.Svg;

public class BoxModel
{
    public int X { get; set; } = 0;

    public int Y { get; set; } = 0;

    public int Width { get; set; } = 0;

    public int Height { get; set; } = 0;

    public string Name { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;    

    public void Debug()
    {
        Console.WriteLine($"Name: {this.Name}, X: {this.X}, Y: {this.Y}, Width: {this.Width}, Height: {this.Height}");
    }
}