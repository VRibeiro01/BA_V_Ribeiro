using System;
using System.Linq;
using LaserTagBox.Model.Location.LocationNodes;
using Mars.Common.Core.Collections;
using Mars.Common.Data;
using Mars.Common.IO.Mapped.Collections;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Refugee;

public class RefugeeLayer : AbstractLayer
{
    private System.Collections.Generic.Dictionary<String,int> InitDistributionData { get; set; }
    public IGeoEnvironment Environment => new EnvironmentImpl();
    
    public List<RefugeeAgent> RefugeeAgents = new List<RefugeeAgent>();

    
 


    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);

        
       
        
        InitDistributionData = layerInitData.LayerInitConfig.Inputs.Import()
            .OfType<IStructuredData>()
            .ToDictionary(data => Convert.ToString(data.Data["Nahya"]), data=> Convert.ToInt32(data.Data["IDPs"]));

        
            IAgentManager agentManager = layerInitData.Container.Resolve<IAgentManager>();
        



        DistributeRefs(RefugeeAgents, agentManager);

       InitSocialNetworks();
         
       Console.WriteLine(RefugeeAgents.Count + " refugee agent(s) spawned");

        return true;
    }

    private void InitSocialNetworks()
    {
        foreach (var agent in RefugeeAgents)
        {
            agent.InitSocialLinks();
        }
    }

    
    private void DistributeRefs(List<RefugeeAgent> refugeeAgentsSpawned, IAgentManager agentManager)
    {
        
        foreach (var nodePopPair in InitDistributionData)
        {
            var agents = agentManager.Spawn<RefugeeAgent, RefugeeLayer>(null,
                agent => agent.Spawn(Environment.GetLocationByName(nodePopPair.Key))).Take(nodePopPair.Value);
            foreach (var agent in agents)
            {
                Environment.GetEnvironment().Insert(agent);
                refugeeAgentsSpawned.Add(agent);
            }
           
           


        }
    }
}