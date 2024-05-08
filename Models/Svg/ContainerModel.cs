using Newtonsoft.Json;

namespace Graph.Models.Svg;

public class ContainerModel
{
    public List<BoxModel> Boxes { get; set; } = new List<BoxModel>();
    public List<ArcModel> Arcs { get; set; } = new List<ArcModel>();
} 