using System;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Location;

public class ConflictLayer : VectorLayer<Conflict>
{
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Console.WriteLine(Entities.Count() + " Conflict events created!");
        return true;
    }

    /// <summary>
    /// Return an array with Conflict objects. If there are no conflicts, the array will be empty.
    /// </summary>
    /// <returns></returns>
    public Conflict[] GetConflicts()
    {
        return Entities.ToArray();
    }
}