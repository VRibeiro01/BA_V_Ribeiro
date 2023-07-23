using System;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Location.Conflict;

public class ConflictLayer : VectorLayer<Conflict>
{
    public static ConflictLayer CreateInstance()
    {
        return new ConflictLayer();
    }


    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Console.WriteLine(Entities.Count() + " Conflict events created!");
        return true;
    }
    
    /// <summary>
    /// Return an array with Geometry objects. If there are no conflicts, the array will be empty.
    /// </summary>
    /// <returns></returns>
    public  Geometry[] GetConflictCoordinates()
    {
        return  Entities.Select(con => con.GetCoordinates()).ToArray();
    }
    
}