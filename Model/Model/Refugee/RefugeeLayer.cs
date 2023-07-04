using System;
using System.Linq;
using Mars.Common.Data;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace RefugeeSimulation.Model.Model.Refugee;

public class RefugeeLayer : AbstractLayer
{
    
    
    
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);
       IAgentManager agentManager =  layerInitData.Container.Resolve<IAgentManager>();
       var refugeeGroupsSpawned = agentManager.Spawn<SingleRefugeeGroup, RefugeeLayer>().ToList();
       
       Console.WriteLine(refugeeGroupsSpawned.Count + " refugee agent(s) spawned");

        return true;
    }
}