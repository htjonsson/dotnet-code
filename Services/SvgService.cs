using System.Text;
using Graph.Models.Svg;

namespace Graph.Services;

public class SvgService
{
    private static string Indent1 = "    ";
    private static string Indent2 = "        ";
    private static string Indent3 = "            ";

    private List<string> Body { get; set; } = new List<string>();
    private List<string> Styles { get; set; } = new List<string>();

    public void Add(BoxModel box)
    {
        AddTextBox(box.X, box.Y, box.Width, box.Height, box.Name, box.Label);
    }

    public void AddTextBox(int x, int y, int width, int height, string title, string subtitle = "")
    {
        this.AddBox(x, y, width, height);

        this.AddTextMiddle(x, y, width, 14, title);
        
        if (!string.IsNullOrEmpty(subtitle))
        {
            this.AddTextMiddle(x, y, width, 34, subtitle);
        }
    }

    private void AddBox(int x, int y, int width, int height, bool shadow = true)
    {
        string style = "fill:rgb(255,255,255);stroke:rgb(0,0,0);stroke-width:1;";

        if (shadow)
            style += "filter: drop-shadow(rgba(0, 0, 0, 0.25) 2px 3px 2px);";

        style += "fill: rgb(254, 245, 219);stroke: rgb(228, 220, 197);stroke-width: 1;";

        Body.Add($"<rect x=\"{x}\" y=\"{y}\" width=\"{width}\" height=\"{height}\" style=\"{style}\" />");
    }
    
    private void AddTextMiddle(int x, int y, int width, int height, string text)
    {
        Body.Add($"<text x=\"{x + width / 2}\" y=\"{y + height}\" font-family=\"Consolas, Courier-New, monospace\" font-size=\"10px\" text-anchor=\"middle\">{text}</text>");
    }

    public void Add(ArcModel arc)
    {
        AddArc(arc.X1, arc.Y1, arc.X2, arc.Y2);
    }

    public void AddArc(int x1, int y1, int x2, int y2)
    {
        // Straight line down
        if ((x2 - x1) == 0)
        {
            AddLine(x1, y1, x2, y2);
            return;
        }

        int delta = (y2 - y1) / 2;

        AddLine(x1, y1, x1, y1 + delta);
        AddLine(x2, y2, x2, y2 - delta);
        AddLine(x1, y1 + delta, x2, y2 - delta);
    }

    public void AddLine(int x1, int y1, int x2, int y2, string color="black")
    {
        string filter = "filter: drop-shadow(rgba(0, 0, 0, 0.25) 2px 3px 2px);";

        Body.Add($"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" stroke=\"{color}\" style=\"{filter}\" />");
    }

    public void DefaultStyle()
    {
        /*
        <svg xmlns="http://www.w3.org/2000/svg" width="1000" height="1000">
            <defs>
                <style>
                    @import url("https://fonts.googleapis.com/css?family=Roboto:400,400i,700,700i");
                </style>
            </defs>
            <style><![CDATA[svg text{strStylesoke:none}]]></style>
        */

        Styles.Add(Indent1 + "<defs>");

        Styles.Add(Indent2 + "<style>");
        Styles.Add(Indent3 + $"@import url(\"https://fonts.googleapis.com/css?family=Roboto:400,400i,700,700i\");");
        Styles.Add(Indent2 + "</style>");

        Styles.Add(Indent2 + "<filter id=\"shadow\">");
        Styles.Add(Indent2 + "<feDropShadow dx=\"0.2\" dy=\"0.4\" stdDeviation=\"0.2\" />");
        Styles.Add(Indent2 + "</filter>");

        Styles.Add(Indent1 + "</defs>");
        Styles.Add(Indent1 + "<style><![CDATA[svg text{stroke:none}]]></style>");
    }

    public string ToString(int width, int height)
    {
        this.DefaultStyle();

        var stringBuilder = new StringBuilder();

        // Open SVG
        stringBuilder.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\">");

        foreach(var text in Styles)
        {
            stringBuilder.AppendLine(Indent1 + text);
        }

        foreach(var text in Body)
        {
            stringBuilder.AppendLine(Indent1 + text);
        }

        // Close SVG
        stringBuilder.AppendLine("</svg> ");

        return stringBuilder.ToString();
    }
} 