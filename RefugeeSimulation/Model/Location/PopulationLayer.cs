using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Common.Data;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Location.LocationNodes;

public class PopulationLayer : AbstractLayer
{
    public Dictionary<string, int> SyriaPopulationData { get; set; }
    
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
        
        SyriaPopulationData = layerInitData.LayerInitConfig.Inputs.Import()
            .OfType<IStructuredData>()
            .ToDictionary(data => Convert.ToString(data.Data["Region"]), data => Convert.ToInt32(data.Data["Total_population"]));
        return true;
    }
}