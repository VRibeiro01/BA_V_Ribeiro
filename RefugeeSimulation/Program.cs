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
            RefugeeLayer refugeeLayer = loopResults.Model.Layers.Values.OfType<RefugeeLayer>().First();
       
            if (RefugeeAgent.Validate)
            {
                Validation.NumRuns = (int)loopResults.Iterations;
                Validation.FillRoutes(refugeeLayer.RefugeeAgents);
                Validation.Print();
            }

            
            // Feedback to user that simulation run was successful
            Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        }
    }
}