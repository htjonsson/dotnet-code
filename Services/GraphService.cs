using Newtonsoft.Json;
using System.Collections.Generic;
using Graph.Models;
using Graph.Models.Svg;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Graph.Services;

public class GraphService
{
    private List<NodeModel> Nodes { get; set; } = new List<NodeModel>();
    private NodeModel _RootNode { get; set; }

    public GraphService(NodeModel root)
    {
        _RootNode = root;
    }

    public NodeModel Add(NodeModel parent, NodeModel child)
    {
        parent.Children.Add(child);

        return child;
    }

    #region Layout Elements

    public void DoLayout(int space = 40)
    {
        if (_RootNode != null)
        {
            // Align vertical
            VerticalAlign(_RootNode, 1);

            // Add dummy nodes for alignments
            AddDummyNodes();

            // Set the size of node
            NodeAlign(_RootNode);
            
            // Set the sae of the Child nodes
            ContainerAlign(_RootNode, space);
            
            // Shift the nodes horizontal
            SetLocation(space);
        }
    }

    private void PrintNode(NodeModel node)
    {
        System.Console.WriteLine($"Depth {node.Layout.Depth}, X: {node.Layout.X}, Y: {node.Layout.Y}, Offset: {node.Layout.Offset}, Total: {node.Layout.SizeOfChildren}");
    }

    private void AddDummyNodes()
    {
        var nodes = ToList();

        int max = nodes.Max(node => node.Layout.Depth);

        System.Console.WriteLine($"Depth: {max}");

        // Get all nodes that don't reach the bottom
        var list = nodes.Where(node => node.Layout.Depth != max && node.Children.Count == 0);

        foreach(var item in list)
        {
            AddDummyNodes(item, max);
        }
    }

    private void AddDummyNodes(NodeModel node, int max)
    {
        if (node.Layout.Depth >= max)
            return;

        var dummyNode = new NodeModel("{DUMMY}");
        dummyNode.Layout.Depth = node.Layout.Depth+1;

        node.Children.Add(dummyNode);

        AddDummyNodes(dummyNode, max);
    }

    private void SetLocation(int space = 40)
    {
        var offset = new Dictionary<int, int>();
        var nodes = this.ToList(); 

        foreach(var node in nodes)
        {
            // Set the Y position
            node.Layout.Y = (node.Layout.Depth * 2) * space;

            // Set the X position
            if (false == offset.ContainsKey(node.Layout.Depth))
                offset.Add(node.Layout.Depth, 0);

            int currentOffset = offset[node.Layout.Depth];

            node.Layout.Offset = currentOffset;

            node.Layout.X = currentOffset + (node.Layout.SizeOfChildren/2 - node.Layout.Width/2);
        
            offset[node.Layout.Depth] += node.Layout.SizeOfChildren + space;

            System.Console.WriteLine($"Depth: {node.Layout.Depth}, Offset: {node.Layout.Offset}/{offset[node.Layout.Depth]},  X: {node.Layout.X}");
        }
    }

    private void OffsetAlign(Dictionary<int, int> dict, NodeModel node, int space = 40)
    {
        node.Layout.Y = (node.Layout.Depth * 2) * space;
        node.Layout.X = node.Layout.Offset + node.Layout.SizeOfChildren/2 - node.Layout.Width/2;

        PrintNode(node);

        if (false == dict.ContainsKey(node.Layout.Depth))
        {
            dict.Add(node.Layout.Depth, 0);
        }
        else 
        {
            int currentOffset = dict[node.Layout.Depth] + node.Layout.SizeOfChildren + space;
            dict[node.Layout.Depth] = currentOffset;
            node.Layout.Offset = currentOffset;

            System.Console.WriteLine($"Layout -> Depth: {node.Layout.Depth} Offset: {node.Layout.Offset} Total: {node.Layout.SizeOfChildren}");
        }

        foreach (var child in node.Children)
        {
            child.Layout.X = child.Layout.Offset + child.Layout.SizeOfChildren/2 - node.Layout.Width/2;

            OffsetAlign(dict, child, space);
        }
    }

    private List<NodeModel> ToList()
    {
        return ToList(_RootNode, new List<NodeModel>());
    }

    private List<NodeModel> ToList(NodeModel node, List<NodeModel> nodes)
    {
        nodes.Add(node);

        foreach(var child in node.Children)
        {
            ToList(child, nodes);
        }

        return nodes;
    }
        
    private void VerticalAlign(NodeModel node, int depth)
    {
        foreach(var child in node.Children)
        {
            child.Layout.Depth = depth;

            VerticalAlign(child, depth + 1);
        }
    }

    private void ContainerAlign(NodeModel node, int space)
    {
        // Loop through all children
        foreach(var child in node.Children)
        {
            ContainerAlign(child, space);
        }

        if (node.Children.Count != 0)
        {
            // Get the children widths
            int width = 0;
            foreach(var child in node.Children)
            {
                width += child.Layout.SizeOfChildren;
            }
            node.Layout.SizeOfChildren = width + ((node.Children.Count - 1) * space);
        }
    }

    private void NodeAlign(NodeModel node)
    {
        // If end element, then size it itself
        if (node.Children.Count == 0)
        {
            node.Layout.SizeOfChildren = node.Layout.Width;
        }
        else
        {
            node.Layout.SizeOfChildren = 0;
        }

        // Loop through all children
        foreach(var child in node.Children)
        {
            NodeAlign(child);
        }
    }

    #endregion    

    #region Create Drawing Elements

    public ContainerModel ToContainer(int blockSize = 40)
    {
        var container = new ContainerModel();

        container.Boxes = CreateBoxes(blockSize);
        container.Arcs = CreateArcs(container.Boxes);

        return container;  
    }

    private List<BoxModel> CreateBoxes(int blockSize = 40)
    {
        var result = new List<BoxModel>();

        foreach(var item in ToList().Where(node => node.Name != "{DUMMY}"))
        {
            var node = new BoxModel();

            node.Name = item.Name;
            
            node.Width = item.Layout.Width;
            node.Height = item.Layout.Height;

            node.X = item.Layout.X;
            node.Y = item.Layout.Y;

            result.Add(node);
        }

        return result;
    }

    private List<NodeModel> GetParents(NodeModel node)
    {
        return ToList().Where(x => x.Children.Any(a => a.Name == node.Name)).ToList();
    }

    private List<ArcModel> CreateArcs(List<BoxModel> boxes)
    {
        var result = new List<ArcModel>();

        foreach(var item in ToList().Where(node => node.Name != "{DUMMY}"))
        {
            foreach(var parent in GetParents(item))
            {
                var parentBox = boxes.FirstOrDefault(x => x.Name == parent.Name);
                var childBox = boxes.FirstOrDefault(x => x.Name == item.Name);

                if (parentBox != null && childBox != null)
                {
                    result.Add(GetArc(parentBox, childBox));
                }
            }
        }

        return result;
    }

    private ArcModel GetArc(BoxModel parent, BoxModel child)
    {
        return new ArcModel()
        {
            X1 = parent.X + parent.Width / 2, // center of top
            Y1 = parent.Y + parent.Height,
            X2 = child.X + child.Width / 2, // center of child
            Y2 = child.Y
        };
    }

    #endregion

    #region Debug Information

    public void Debug()
    {
        Console.WriteLine("----------------------------------------------------------");

        if (_RootNode != null)
        {
            Debug(_RootNode);
        }
    }

    private void Debug(NodeModel node)
    {    
        System.Console.WriteLine($" {node.Name} X: {node.Layout.X} Y: {node.Layout.Y} Width: {node.Layout.Width} Height: {node.Layout.Height} Depth: {node.Layout.Depth} SizeOfChildren: {node.Layout.SizeOfChildren} Offset: {node.Layout.Offset}");
        
        foreach (var child in node.Children)
        {
            Debug(child);
        }
    }

    #endregion
}