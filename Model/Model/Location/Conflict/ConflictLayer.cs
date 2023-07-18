using System;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Model.Location.Conflict;

public class ConflictLayer : VectorLayer<Conflict>
{
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Console.WriteLine(Entities.Count() + " Conflict events created!");
        return true;
    }
    
    public Geometry[] GetConflictCoordinates()
    {
        return null;
    }
    
}