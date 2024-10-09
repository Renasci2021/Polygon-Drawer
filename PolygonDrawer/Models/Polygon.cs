namespace PolygonDrawer.Models;

public class Polygon
{
    private int _currentRing = -1;  // -1 means outer ring, 0 means first inner ring, etc.
    private bool _isAddingVertices = false;

    public List<Vertex> OuterVertices { get; } = [];
    public List<List<Vertex>> InnerVertices { get; } = [];

    public Vertex FirstVertexInCurrentRing => _currentRing == 0 ? OuterVertices[0] : InnerVertices[_currentRing][0];

    public void AddVertex(Vertex vertex)
    {
        _isAddingVertices = true;

        if (_currentRing == -1)
        {
            OuterVertices.Add(vertex);
        }
        else
        {
            if (_currentRing == InnerVertices.Count)
            {
                InnerVertices.Add([]);
            }
            InnerVertices[_currentRing].Add(vertex);
        }
    }

    public void CloseRing()
    {
        if (!_isAddingVertices)
        {
            return;
        }

        if (_currentRing == -1 && OuterVertices.Count < 3)
        {
            throw new InvalidOperationException("A polygon must have at least 3 outer vertices.");
        }
        if (_currentRing >= 0 && InnerVertices[_currentRing].Count < 3)
        {
            throw new InvalidOperationException("A polygon must have at least 3 inner vertices.");
        }

        _isAddingVertices = false;
        _currentRing++;
    }

    public void CompletePolygon()
    {
        if (_isAddingVertices)
        {
            throw new InvalidOperationException("Cannot complete polygon while adding vertices.");
        }

        // Sort vertices in each ring
        SortVertices(OuterVertices, isClockwise: true);
        foreach (var innerVertices in InnerVertices)
        {
            SortVertices(innerVertices, isClockwise: false);
        }

        // Connect vertices in each ring
        ConnectVertices(OuterVertices);
        foreach (var innerVertices in InnerVertices)
        {
            ConnectVertices(innerVertices);
        }
    }

    private static void SortVertices(List<Vertex> vertices, bool isClockwise)
    {
        Point centroid = CalculateCentroid(vertices);
        vertices.Sort((a, b) =>
        {
            var angleA = Math.Atan2(a.Y - centroid.Y, a.X - centroid.X);
            var angleB = Math.Atan2(b.Y - centroid.Y, b.X - centroid.X);
            return isClockwise ? angleA.CompareTo(angleB) : angleB.CompareTo(angleA);
        });
    }

    private static Point CalculateCentroid(List<Vertex> vertices)
    {
        double x = vertices.Sum(v => v.X) / vertices.Count;
        double y = vertices.Sum(v => v.Y) / vertices.Count;
        return new Point(x, y);
    }

    private static void ConnectVertices(List<Vertex> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].Next = vertices[(i + 1) % vertices.Count];
        }
    }
}