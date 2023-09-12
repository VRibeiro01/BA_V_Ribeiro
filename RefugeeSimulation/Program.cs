using System;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.Camps;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Location.LocationNodes;
using LaserTagBox.Model.Refugee;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using ServiceStack;

namespace LaserTagBox
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // Simulation mode Turkey: Movement of Syrian Refugees over Turkish territory
            // Simulation mode Syria: Movement of Syrian IDPs over Syrian territory
            string simulationMode = "Syria";
            
            // the scenario consists of the model (represented by the model description)
            // and the simulation configuration (see config.json files)

            // Create a new model description that holds all parts of the model (agents, entities, layers)
            var description = new ModelDescription();

            description.AddLayer<ConflictLayer>();
            description.AddLayer<CampLayer>();
            description.AddLayer<NodeLayer>();
            description.AddLayer<RefugeeLayer>();
            description.AddLayer<BorderCrossingScheduleLayer>();
            description.AddAgent<RefugeeAgent, RefugeeLayer>();


            // scenario definition
            var file = File.ReadAllText("config_turkey.json");
            if (simulationMode.EqualsIgnoreCase("Turkey"))
            {
                Console.WriteLine("Simulation starting in Turkey mode ...");
            } else if (simulationMode.EqualsIgnoreCase("Syria"))
            {
                file = File.ReadAllText("config_syria.json");
                Console.WriteLine("Simulation starting in Syria mode ...");
            }
            else
            {
                Console.WriteLine("Invalid simulation mode input. Simulation starting in Turkey mode per default ...");
            }

            var config = SimulationConfig.Deserialize(file);
            
            // Instantiate and configure simulation components
            var task = SimulationStarter.Start(description, config);

            // Run simulation
            var loopResults = task.Run();
            
            
            // Collect results for output files
            RefugeeLayer refugeeLayer = loopResults.Model.Layers.Values.OfType<RefugeeLayer>().First();
            NodeLayer nodeLayer = loopResults.Model.Layers.Values.OfType<NodeLayer>().First();
            Validation.NumSimRuns++;
            Validation.NumRuns = (int) loopResults.Iterations;
            Validation.FillRoutes(refugeeLayer.RefugeeAgents);
            Validation.Print();
            if(simulationMode.EqualsIgnoreCase("Syria"))  Validation.FillSyrianDistrictsPop(nodeLayer.GetEntities().ToList());
            else Validation.FillTurkishDistrictsPop(nodeLayer.GetEntities().ToList());
         

            if (RefugeeAgent.Validate)
            {
                for (int i = 0; i < 4; i++)
                {
                    refugeeLayer = loopResults.Model.Layers.Values.OfType<RefugeeLayer>().First();
                    nodeLayer = loopResults.Model.Layers.Values.OfType<NodeLayer>().First();
                    Validation.NumSimRuns++;
                    Validation.NumRuns = (int) loopResults.Iterations;
                    Validation.FillRoutes(refugeeLayer.RefugeeAgents);
                    if(simulationMode.EqualsIgnoreCase("Syria"))  Validation.FillSyrianDistrictsPop(nodeLayer.GetEntities().ToList());
                    else Validation.FillTurkishDistrictsPop(nodeLayer.GetEntities().ToList());
                    
                    task = SimulationStarter.Start(description, config);
                    loopResults = task.Run();
                }
                Validation.CalcAverageDistribution();
                if(simulationMode.EqualsIgnoreCase("Syria"))  Validation.WriteToFileSyria("");
                else Validation.WriteToFileTurkey("");
                
            }


            // Feedback to user that simulation run was successful
            Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        }
    }
}