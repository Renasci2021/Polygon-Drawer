using Microsoft.UI.Composition;
using PolygonDrawer.Models;
using Vector = PolygonDrawer.Models.Vector;

namespace PolygonDrawer.Algorithms;

public class PolygonClipper
{
    public static Polygon Clip(Polygon mainPolygon, Polygon clipPolygon)
    {
        var intersections = FindIntersections(mainPolygon, clipPolygon);
        AddIntersectionVertices(intersections);
        return BuildResultPolygon(intersections);
    }

    private static List<Vertex> FindIntersections(Polygon mainPolygon, Polygon clipPolygon)
    {
        var intersections = new List<Vertex>();

        // 遍历主多边形和裁剪多边形的所有边
        foreach (var mainVertex in mainPolygon.AllVertices)
        {
            foreach (var clipVertex in clipPolygon.AllVertices)
            {
                if (TryCalculateIntersection(mainVertex.Value, mainVertex.Next!.Value, clipVertex.Value, clipVertex.Next!.Value, out var intersection))
                {
                    intersection!.FromMainVertex = mainVertex;
                    intersection!.FromClipVertex = clipVertex;
                    intersections.Add(intersection!);
                }
            }
        }

        return intersections;
    }

    private static void AddIntersectionVertices(List<Vertex> intersections)
    {
        // 将交点添加到主多边形和裁剪多边形的点集中
        foreach (var intersection in intersections)
        {
            var mainVertex = intersection;
            var clipVertex = new Vertex(intersection.Value, isEntryPoint: true)
            {
                FromClipVertex = intersection.FromClipVertex,
                CorrespondingVertex = mainVertex,
            };
            mainVertex.CorrespondingVertex = clipVertex;

            ConnectIntersection(mainVertex, mainVertex.FromMainVertex!);
            ConnectIntersection(clipVertex, clipVertex.FromClipVertex!);
        }
    }

    private static Polygon BuildResultPolygon(List<Vertex> intersections)
    {
        var resultPolygon = new Polygon();

        // 遍历所有未处理的交点
        foreach (var intersection in intersections)
        {
            if (intersection.IsProcessed)
            {
                continue;
            }

            // 从交点出发，按照交点的入点标记，连接所有交点，形成一个新的多边形
            var currentVertex = intersection;
            bool isInMainPolygon = true;
            do
            {
                resultPolygon.AddVertex(currentVertex);
                currentVertex.IsProcessed = true;
                if (currentVertex.IsIntersection)
                {
                    currentVertex.CorrespondingVertex!.IsProcessed = true;
                }

                if (currentVertex.IsIntersection &&
                    currentVertex.IsEntryPoint != isInMainPolygon)
                {
                    currentVertex = currentVertex.CorrespondingVertex!;
                    isInMainPolygon = !isInMainPolygon;
                }
                currentVertex = currentVertex.Next!;
            } while (!currentVertex.IsProcessed);

            resultPolygon.CloseRing();
        }

        return resultPolygon;
    }

    private static bool TryCalculateIntersection(Vector mainStart, Vector mainEnd, Vector clipStart, Vector clipEnd, out Vertex? intersection)
    {
        intersection = null;

        // 计算主多边形和裁剪多边形的边的方向向量
        var mainDirection = mainEnd - mainStart;
        var clipDirection = clipEnd - clipStart;

        // 计算行列式
        var determinant = mainDirection.X * clipDirection.Y - mainDirection.Y * clipDirection.X;
        if (Math.Abs(determinant) < 1e-6)
        {
            return false;
        }

        // 计算参数 t 和 u （参数方程的比例系数）
        var t = (clipStart.X - mainStart.X) * (clipEnd.Y - clipStart.Y) - (clipStart.Y - mainStart.Y) * (clipEnd.X - clipStart.X);
        t /= (mainEnd.X - mainStart.X) * (clipEnd.Y - clipStart.Y) - (mainEnd.Y - mainStart.Y) * (clipEnd.X - clipStart.X);
        var u = (clipStart.X - mainStart.X) * (mainEnd.Y - mainStart.Y) - (clipStart.Y - mainStart.Y) * (mainEnd.X - mainStart.X);
        u /= (mainEnd.X - mainStart.X) * (clipEnd.Y - clipStart.Y) - (mainEnd.Y - mainStart.Y) * (clipEnd.X - clipStart.X);

        // t 和 u 的取值范围均为 [0, 1] 表示交点在线段内
        if (t < 0 || t > 1 || u < 0 || u > 1)
        {
            return false;
        }

        // 计算交点坐标，并判断是否为入点
        bool isEntryPoint = Vector.CrossProduct(mainDirection, clipDirection) > 0;
        intersection = new(mainStart + t * mainDirection, isEntryPoint);

        return true;
    }

    public static void ConnectIntersection(Vertex intersection, Vertex startVertex)
    {
        // 将交点插入到正确的位置
        var currentVertex = startVertex;
        var endVertex = startVertex.Next!;
        while (endVertex.IsIntersection)
        {
            endVertex = endVertex.Next!;
        }

        var currentDistance = (intersection.Value - startVertex.Value).SquaredLength;
        while (currentVertex.Next != endVertex)
        {
            var nextDistance = (currentVertex.Next!.Value - startVertex.Value).SquaredLength;
            if (nextDistance > currentDistance)
            {
                break;
            }
            currentVertex = currentVertex.Next!;
            currentDistance = nextDistance;
        }

        intersection.Next = currentVertex.Next;
        currentVertex.Next = intersection;
    }
}