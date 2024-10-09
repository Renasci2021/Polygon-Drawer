namespace PolygonDrawer.Models;

public class Vertex(double x, double y, bool isIntersection = false) : Point(x, y)
{
    public Vertex? Next { get; set; }

    public bool IsIntersection { get; } = isIntersection;
}