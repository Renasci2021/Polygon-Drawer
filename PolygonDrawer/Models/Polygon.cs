namespace PolygonDrawer.Models;

public class Polygon
{
    private int _currentRing = -1;  // -1 means outer ring, 0 means first inner ring, etc.
    private bool _isAddingVertices = false;

    public List<Vertex> OuterVertices { get; } = [];
    public List<List<Vertex>> InnerVertices { get; } = [];

    public List<Vertex> AllVertices => AllRings.SelectMany(vertices => vertices).ToList();

    private List<List<Vertex>> AllRings => [OuterVertices, .. InnerVertices];

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
        SortVertices(OuterVertices, isClockwise: false);
        foreach (var innerVertices in InnerVertices)
        {
            SortVertices(innerVertices, isClockwise: true);
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
        Vector centroid = CalculateCentroid(vertices);
        vertices.Sort((a, b) =>
        {
            var angleA = Math.Atan2(a.Value.Y - centroid.Y, a.Value.X - centroid.X);
            var angleB = Math.Atan2(b.Value.Y - centroid.Y, b.Value.X - centroid.X);
            return isClockwise ? angleA.CompareTo(angleB) : angleB.CompareTo(angleA);
        });
    }

    private static Vector CalculateCentroid(List<Vertex> vertices)
    {
        double x = vertices.Sum(v => v.Value.X) / vertices.Count;
        double y = vertices.Sum(v => v.Value.Y) / vertices.Count;
        return new Vector(x, y);
    }

    private static void ConnectVertices(List<Vertex> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].Next = vertices[(i + 1) % vertices.Count];
        }
    }
}