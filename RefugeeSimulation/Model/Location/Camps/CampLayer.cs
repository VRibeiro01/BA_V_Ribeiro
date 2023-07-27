using System;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Location.Camps;

public class CampLayer : VectorLayer<Camp>
{
   
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Console.WriteLine(Entities.Count() + " Camps created!");
        return true;
    }

    public Geometry[] GetCamps()
    {
        return Entities.Select(camp => camp.GetCoordinates()).ToArray();
    }
}