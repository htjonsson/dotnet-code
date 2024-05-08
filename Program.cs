using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Graph.Services;
using Graph.Models;
using Graph.Models.Svg;

namespace Graph;

class Program
{
    static void SaveToFile(ContainerModel model)
    {
        var service = new SvgService();

        foreach(var arc in model.Arcs)
        {
            service.Add(arc);
        }

        foreach(var box in model.Boxes)
        {
            service.Add(box);
        }

        System.IO.File.WriteAllText(Guid.NewGuid().ToString() + ".svg", service.ToString(3000, 3000));
    }

    static void Main(string[] args)
    {
        // Node(s)

        var applicationShellWidgetClassNode = new NodeModel("applicationShellWidgetClass");

        var xmVaCreateMainWindow = new NodeModel("xmVaCreateMainWindow");
        var xmVaCreateManagedFrame = new NodeModel("xmVaCreateManagedFrame");
        var xmVaCreateManagedPanedWindow = new NodeModel("xmVaCreateManagedPanedWindow");
        var xmVaCreateManagedPanedWindow2 = new NodeModel("xmVaCreateManagedPanedWindow2");
        var xmVaCreateScrolledWindow = new NodeModel("xmVaCreateScrolledWindow");
        var xmVaCreateManagedList = new NodeModel("xmVaCreateManagedList");
        var xmVaCreateManagedScrolledWindow = new NodeModel("xmVaCreateManagedScrolledWindow");
        var xmVaCreateManagedScrollBar = new NodeModel("xmVaCreateManagedScrollBar");
        var xmVaCreateManagedDrawingArea = new NodeModel("xmVaCreateManagedDrawingArea");
        var xmVaCreateManagedFrame2 = new NodeModel("xmVaCreateManagedFrame2");
        var xmVaCreateManagedLabelGadget = new NodeModel("xmVaCreateManagedLabelGadget");

        
        // AutoLayoutService

        var layout = new AutoLayoutService(applicationShellWidgetClassNode);

        layout.Add(applicationShellWidgetClassNode, xmVaCreateMainWindow);
        layout.Add(xmVaCreateMainWindow, xmVaCreateManagedFrame);    
        layout.Add(xmVaCreateManagedFrame, xmVaCreateManagedPanedWindow);
        layout.Add(xmVaCreateManagedPanedWindow, xmVaCreateManagedPanedWindow2);
        layout.Add(xmVaCreateManagedPanedWindow2, xmVaCreateScrolledWindow);
        layout.Add(xmVaCreateScrolledWindow, xmVaCreateManagedList);
        layout.Add(xmVaCreateManagedPanedWindow2, xmVaCreateManagedScrolledWindow);
        layout.Add(xmVaCreateManagedScrolledWindow, xmVaCreateManagedScrollBar);
        layout.Add(xmVaCreateManagedScrolledWindow, xmVaCreateManagedDrawingArea);
        layout.Add(xmVaCreateMainWindow, xmVaCreateManagedFrame2);
        
        var n1 = layout.Add(xmVaCreateManagedFrame2, xmVaCreateManagedLabelGadget);
        var n2 = layout.Add(n1, new NodeModel());
        var n3 = layout.Add(n1, new NodeModel());
        var n4 = layout.Add(n1, new NodeModel());
        var n5 = layout.Add(n3, new NodeModel());
        var n6 = layout.Add(n3, new NodeModel());
        
        layout.DoLayout();

        var cont = layout.ToContainer();

        layout.Debug();
        
        SaveToFile(cont);
    }
}