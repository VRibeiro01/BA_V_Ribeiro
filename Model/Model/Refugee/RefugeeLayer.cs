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
    public List<RefugeeAgent> RefugeeAgents = new List<RefugeeAgent>();
    
    
    
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
       
       
       
       distributeRefs(RefugeeAgents, agentManager);
         
       Console.WriteLine(RefugeeAgents.Count + " refugee agent(s) spawned");

        return true;
    }

    private void distributeRefs(List<RefugeeAgent> refugeeAgentsSpawned, IAgentManager agentManager)
    {
        
        foreach (var nodePopPair in InitDistributionData)
        {
            var spawnCity = nodeLayer.GetCityByName(nodePopPair.Key);
            refugeeAgentsSpawned.AddRange(agentManager.Spawn<RefugeeAgent, RefugeeLayer>(null,
                agent => agent.Spawn(spawnCity)).Take(nodePopPair.Value).ToList());

          //TODO refactor to object attribute and change name to refPop
            spawnCity.VectorStructured.Data["Residents"] = nodePopPair.Value;

        }
    }
}