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
    private Dictionary<String, int> InitDistributionData { get; set; }

    [PropertyDescription] public NodeLayer NodeLayer { get; set; }
    public GeoHashEnvironment<RefugeeAgent> Environment { get; set; }

    public List<RefugeeAgent> RefugeeAgents;

    public IAgentManager AgentManager;


    [PropertyDescription] public int RefsToSpawnAtBorder { get; set; }


    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);


        InitDistributionData = layerInitData.LayerInitConfig.Inputs.Import()
            .OfType<IStructuredData>()
            .ToDictionary(data => Convert.ToString(data.Data["Region"]), data => Convert.ToInt32(data.Data["IDPs"]));

        Environment = NodeLayer.GetEnvironment();

        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        RefugeeAgents = new List<RefugeeAgent>();

        InitRefs();
        if (RefugeeAgent.Validate)
        {
            Validation.FillTurkishDistrictsInitPop(NodeLayer.GetEntities());
        }


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

    public void InitRefs()
    {
        var newRefs = new List<RefugeeAgent>();
        foreach (var nodePopPair in InitDistributionData)
        {
            var provinces = NodeLayer.GetLocationsInProvince(nodePopPair.Key);
            foreach (var province in provinces)
            {
                newRefs.AddRange(AgentManager.Spawn<RefugeeAgent, RefugeeLayer>(null,
                        agent => agent.Spawn(province))
                    .Take(nodePopPair.Value / provinces.Count / 5000
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
                .Take(RefsToSpawnAtBorder
                ));
        }

        PostSpawnWork(newRefs);
    }

    private void PostSpawnWork(List<RefugeeAgent> newRefs)
    {
        RefugeeAgents.AddRange(newRefs);
        Console.WriteLine(newRefs.Count + " refugee agent(s) spawned");
        if (RefugeeAgent.Validate) Validation.RefsSpawned += newRefs.Count;
        InitSocialNetwork(newRefs);
    }
}