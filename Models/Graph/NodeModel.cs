namespace Graph.Models;

public class NodeModel
{
    public NodeModel(string name = "", int width=200, int height=40)
    {
        Name = name;

        if (string.IsNullOrEmpty(name))
        {
            Name = Guid.NewGuid().ToString();
        }

        this.Layout.Height = height;
        this.Layout.Width = width;
    }

    public string Name { get; set; } = string.Empty;

    public List<NodeModel> Children { get; set; } = new List<NodeModel>();

    public LayoutModel Layout { get; set; } = new LayoutModel();

    public NodeModel Add(NodeModel model)
    {
        this.Children.Add(model);

        return model;
    }
}