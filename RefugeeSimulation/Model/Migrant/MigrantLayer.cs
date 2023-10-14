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

public class MigrantLayer : AbstractLayer
{
    private Dictionary<String, int> RefugeeDistributionData { get; set; }
    
    public GeoHashEnvironment<MigrantAgent> Environment { get; set; }

    public List<MigrantAgent> RefugeeAgents;

    private IAgentManager AgentManager;


    [PropertyDescription] 
    public int NumAgentsToSpawn { get; set; }
    
    
    // -------------------------------------- Layers ----------------------------------------
    [PropertyDescription] public LocationLayer LocationLayer { get; set; }


    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);


        RefugeeDistributionData = layerInitData.LayerInitConfig.Inputs.Import()
            .OfType<IStructuredData>()
            .ToDictionary(data => Convert.ToString(data.Data["Region"]), data => Convert.ToInt32(data.Data["IDPs"]));

        Environment = LocationLayer.GetEnvironment();

        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        RefugeeAgents = new List<MigrantAgent>();

        InitRefs();
        
        
        Validation.FillTurkishDistrictsInitPop(LocationLayer.GetEntities());
        Validation.FillSyrianDistrictsInitPop(LocationLayer.GetEntities());
        
        


        return true;
    }

    private void InitSocialNetwork(List<MigrantAgent> refs)
    {
        foreach (var agent in refs)
        {
            Environment.Insert(agent);
            agent.InitSocialLinks();
        }
    }

    private void InitRefs()
    {
        var newRefs = new List<MigrantAgent>();
        foreach (var nodePopPair in RefugeeDistributionData)
        {
            var provinces = LocationLayer.GetLocationsInProvince(nodePopPair.Key);
            foreach (var province in provinces)
            {
                newRefs.AddRange(AgentManager.Spawn<MigrantAgent, MigrantLayer>(null,
                        agent => agent.Spawn(province))
                    .Take((int) (nodePopPair.Value/1.15/provinces.Count)
                    ));
            }
        }

        PostSpawnWork(newRefs);
    }

    public void SpawnNewRefs()
    {
        var newRefs = new List<MigrantAgent>();
        foreach (var borderCrossingNode in LocationLayer.BorderCrossingNodes)
        {
            newRefs.AddRange(AgentManager.Spawn<MigrantAgent, MigrantLayer>(null,
                    agent => agent.Spawn(LocationLayer.GetLocationByName(borderCrossingNode)))
                .Take(NumAgentsToSpawn
                ));
        }

        PostSpawnWork(newRefs);
    }

    public void SpawnNewIDPs()
    {
        var newRefs = new List<MigrantAgent>();
        var remainingAgentsToSpawn = NumAgentsToSpawn;
        foreach (var locationNode in LocationLayer.EntitiesList)
        {
            var agentsToSpawnAtLocation = (int) (remainingAgentsToSpawn * (locationNode.NormPop+locationNode.NormNumConflicts));
            newRefs.AddRange(AgentManager.Spawn<MigrantAgent, MigrantLayer>(null,
                    agent => agent.Spawn(locationNode))
                .Take(agentsToSpawnAtLocation
                ));
            remainingAgentsToSpawn -= agentsToSpawnAtLocation;

        }
        
        PostSpawnWork(newRefs);
    }

    private void PostSpawnWork(List<MigrantAgent> newRefs)
    {
        RefugeeAgents.AddRange(newRefs);
        Console.WriteLine(newRefs.Count + " agent(s) spawned");
        Validation.RefsSpawned += newRefs.Count;
        InitSocialNetwork(newRefs);
    }
}