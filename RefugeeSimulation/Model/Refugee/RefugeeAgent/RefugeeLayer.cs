using System;
using System.Collections.Generic;
using System.Linq;
using LaserTagBox.Model.Location.LocationNodes;
using Mars.Common.Core.Collections;
using Mars.Common.Data;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Refugee;

public class RefugeeLayer : AbstractLayer
{
    private Dictionary<String, int> RefugeeDistributionData { get; set; }
    
    public GeoHashEnvironment<RefugeeAgent> Environment { get; set; }

    public List<RefugeeAgent> RefugeeAgents;

    private IAgentManager AgentManager;


    [PropertyDescription] 
    public int NumAgentsToSpawn { get; set; }
    
    
    // -------------------------------------- Layers ----------------------------------------
    [PropertyDescription] public NodeLayer NodeLayer { get; set; }


    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);


        RefugeeDistributionData = layerInitData.LayerInitConfig.Inputs.Import()
            .OfType<IStructuredData>()
            .ToDictionary(data => Convert.ToString(data.Data["Region"]), data => Convert.ToInt32(data.Data["IDPs"]));

        Environment = NodeLayer.GetEnvironment();

        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        RefugeeAgents = new List<RefugeeAgent>();

        InitRefs();
        
        
        Validation.FillTurkishDistrictsInitPop(NodeLayer.GetEntities());
        Validation.FillSyrianDistrictsInitPop(NodeLayer.GetEntities());
        
        


        return true;
    }

    private void InitSocialNetwork(List<RefugeeAgent> refs)
    {
        foreach (var agent in refs)
        {
            Environment.Insert(agent);
            agent.InitSocialLinks();
        }
    }

    private void InitRefs()
    {
        var newRefs = new List<RefugeeAgent>();
        foreach (var nodePopPair in RefugeeDistributionData)
        {
            var provinces = NodeLayer.GetLocationsInProvince(nodePopPair.Key);
            foreach (var province in provinces)
            {
                newRefs.AddRange(AgentManager.Spawn<RefugeeAgent, RefugeeLayer>(null,
                        agent => agent.Spawn(province))
                    .Take(nodePopPair.Value/provinces.Count/12
                    ));
            }
        }

        PostSpawnWork(newRefs);
    }

    public void SpawnNewRefs()
    {
        var newRefs = new List<RefugeeAgent>();
        foreach (var borderCrossingNode in NodeLayer.BorderCrossingNodes)
        {
            newRefs.AddRange(AgentManager.Spawn<RefugeeAgent, RefugeeLayer>(null,
                    agent => agent.Spawn(NodeLayer.GetLocationByName(borderCrossingNode)))
                .Take(NumAgentsToSpawn
                ));
        }

        PostSpawnWork(newRefs);
    }

    public void SpawnNewIDPs()
    {
        var newRefs = new List<RefugeeAgent>();
        var remainingAgentsToSpawn = NumAgentsToSpawn;
        foreach (var locationNode in NodeLayer.EntitiesList)
        {
            var agentsToSpawnAtLocation = (int)(remainingAgentsToSpawn * (locationNode.NormPop*locationNode.NormNumConflicts));
            newRefs.AddRange(AgentManager.Spawn<RefugeeAgent, RefugeeLayer>(null,
                    agent => agent.Spawn(locationNode))
                .Take(agentsToSpawnAtLocation
                ));
            remainingAgentsToSpawn -= agentsToSpawnAtLocation;

        }
        
        PostSpawnWork(newRefs);
    }

    private void PostSpawnWork(List<RefugeeAgent> newRefs)
    {
        RefugeeAgents.AddRange(newRefs);
        Console.WriteLine(newRefs.Count + " agent(s) spawned");
        Validation.RefsSpawned += newRefs.Count;
        InitSocialNetwork(newRefs);
    }
}