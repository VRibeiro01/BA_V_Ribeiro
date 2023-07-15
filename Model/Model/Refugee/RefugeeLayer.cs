using System;
using System.Linq;
using LaserTagBox.Model.Model.Location.LocationNodes;
using Mars.Common.Core.Collections;
using Mars.Common.Data;
using Mars.Common.IO.Mapped.Collections;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace RefugeeSimulation.Model.Model.Refugee;

public class RefugeeLayer : AbstractLayer
{
    private System.Collections.Generic.Dictionary<String,int> InitDistributionData { get; set; }
    private NodeLayer nodeLayer;

    public RefugeeLayer(NodeLayer nodeLayer)
    {
        this.nodeLayer = nodeLayer;
    }
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
        
        InitDistributionData = layerInitData.LayerInitConfig.Inputs.Import()
            .OfType<IStructuredData>()
            .ToDictionary(data => Convert.ToString(data.Data["Nahya"]), data=> Convert.ToInt32(data.Data["IDPs"]));
        
        
       IAgentManager agentManager =  layerInitData.Container.Resolve<IAgentManager>();
       var refugeeGroupsSpawned = new List<SingleRefugeeGroup>();
       
       
       foreach(var nodePopPair in InitDistributionData)
       {
          refugeeGroupsSpawned.AddRange( agentManager.Spawn<SingleRefugeeGroup, RefugeeLayer>(null,
               agent => agent.Spawn(nodeLayer.GetCityByName(nodePopPair.Key))).Take(nodePopPair.Value).ToList());
       }
         
       Console.WriteLine(refugeeGroupsSpawned.Count + " refugee agent(s) spawned");

        return true;
    }
}