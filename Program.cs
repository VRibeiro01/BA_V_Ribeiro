using System;
using System.IO;
using LaserTagBox.Model.Model.Location;
using LaserTagBox.Model.Model.Location.Camps;
using LaserTagBox.Model.Model.Location.Conflict;
using LaserTagBox.Model.Model.Location.LocationNodes;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using RefugeeSimulation.Model.Model.Refugee;

namespace RefugeeSimulation
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // the scenario consist of the model (represented by the model description)
            // an the simulation configuration (see config.json)
            
            // Create a new model description that holds all parts of the model (agents, entities, layers)
            var description = new ModelDescription();
            description.AddLayer<RefugeeLayer>();
            description.AddAgent<RefugeeAgent, RefugeeLayer>();
            
            description.AddLayer<NodeLayer>();
            description.AddLayer<ConflictLayer>();
            description.AddLayer<CampLayer>();
           
            
            
            // scenario definition
            // use config.json that holds the specification of the scenario
            var file = File.ReadAllText("config.json");
            var config = SimulationConfig.Deserialize(file);
            
            
            var task = SimulationStarter.Start(description, config);
            
            // Run simulation
            var loopResults = task.Run();
            
            // Feedback to user that simulation run was successful
            Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        }
    }
}