namespace Graph.Models;

public class LayoutModel
{
    public int Depth { get; set; } = 0;    

    public int X { get; set; } = 0; 

    public int Y { get; set; } = 0;  


    public int Width { get; set; } = 160;    

    public int Height { get; set; } = 40;    

    public int SizeOfChildren { get; set; } = 0;     

    public int Offset { get; set; } = 0;       
}