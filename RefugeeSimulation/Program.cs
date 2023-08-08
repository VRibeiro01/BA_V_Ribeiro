using System;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.Camps;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Location.LocationNodes;
using LaserTagBox.Model.Refugee;
using Mars.Components.Starter;
using Mars.Interfaces.Model;

namespace LaserTagBox
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // the scenario consist of the model (represented by the model description)
            // an the simulation configuration (see testConfig.json)
            
            // Create a new model description that holds all parts of the model (agents, entities, layers)
            var description = new ModelDescription();

            description.AddLayer<ConflictLayer>();
            description.AddLayer<CampLayer>();
            description.AddLayer<NodeLayer>();
            description.AddLayer<RefugeeLayer>();
           // description.AddLayer<SpawnScheduleLayer>();
            description.AddAgent<RefugeeAgent, RefugeeLayer>();
            
            
            
            
           
            
            
            // scenario definition
            // use testConfig.json that holds the specification of the scenario
            var file = File.ReadAllText("config.json");
            var config = SimulationConfig.Deserialize(file);
            
            
            var task = SimulationStarter.Start(description, config);
            
            // Run simulation
            var loopResults = task.Run();
            
            var basePath = @"..\..\..\Resources";

            RefugeeLayer refugeeLayer = (RefugeeLayer)loopResults.Model.Layers.Values.First(layer => layer is RefugeeLayer);
            foreach (var agent in refugeeLayer.RefugeeAgents)
            {
                if (!agent.OriginNode.GetName().Equals(agent.LocationName))
                {
                    var agentStat = agent.OriginNode.GetName() + "," + agent.CurrentNode.GetName() + "," + 1 + '\n';
                    File.AppendAllText(Path.Combine(basePath, "agentStats.txt"), agentStat);
                }
            }

            
            // Feedback to user that simulation run was successful
            Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        }
    }
}