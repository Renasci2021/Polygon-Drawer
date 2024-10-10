using PolygonDrawer.Models;
using PolygonDrawer.Algorithms;

namespace PolygonDrawer.Controllers;

public class PolygonProcessor
{
    public enum ProcesssingState
    {
        DrawingMainPolygon,
        DrawingClipPolygon,
        ClippingComplete,
        Done,
    }

    public Polygon MainPolygon { get; private set; } = new();
    public Polygon ClipPolygon { get; private set; } = new();
    public Polygon? ResultPolygon { get; private set; }

    public ProcesssingState State { get; private set; } = ProcesssingState.DrawingMainPolygon;

    public bool AddVertex(Vertex vertex)
    {
        switch (State)
        {
            case ProcesssingState.DrawingMainPolygon:
                MainPolygon.AddVertex(vertex);
                return true;
            case ProcesssingState.DrawingClipPolygon:
                ClipPolygon.AddVertex(vertex);
                return true;
            default:
                return false;
        }
    }

    public bool ClosePolygon()
    {
        switch (State)
        {
            case ProcesssingState.DrawingMainPolygon:
                MainPolygon.CloseRing();
                return true;
            case ProcesssingState.DrawingClipPolygon:
                ClipPolygon.CloseRing();
                return true;
            default:
                return false;
        }
    }

    public void SwitchToNextState()
    {
        switch (State)
        {
            case ProcesssingState.DrawingMainPolygon:
                MainPolygon.CompletePolygon();
                State = ProcesssingState.DrawingClipPolygon;
                break;
            case ProcesssingState.DrawingClipPolygon:
                ClipPolygon.CompletePolygon();
                ResultPolygon = PolygonClipper.Clip(MainPolygon, ClipPolygon);
                State = ProcesssingState.ClippingComplete;
                break;
            default:
                State = ProcesssingState.Done;
                break;
        }
    }

    public void Reset()
    {
        MainPolygon = new();
        ClipPolygon = new();
        ResultPolygon = null;
        State = ProcesssingState.DrawingMainPolygon;
    }
}