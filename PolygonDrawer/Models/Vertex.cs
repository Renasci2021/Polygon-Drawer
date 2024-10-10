namespace PolygonDrawer.Models;

public class Vertex(double x, double y)
{
    public Vector Value { get; } = new(x, y);
    public Vertex? Next { get; set; }

    public bool IsIntersection { get; private set; } = false;
    public bool IsEntryPoint { get; set; } = false;

    public Vertex? CorrespondingVertex { get; set; }
    public Vertex? FromMainVertex { get; set; }
    public Vertex? FromClipVertex { get; set; }

    public bool IsProcessed { get; set; } = false;

    public Vertex(double x, double y, bool isEntryPoint) : this(x, y)
    {
        IsIntersection = true;
        IsEntryPoint = isEntryPoint;
    }

    public Vertex(Vector value) : this(value.X, value.Y) { }
    public Vertex(Vector value, bool isEntryPoint) : this(value.X, value.Y, isEntryPoint) { }
}