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
using SchedulerLayer = LaserTagBox.Model.Refugee.Scheduler.SchedulerLayer;

namespace LaserTagBox
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // Simulation mode Turkey: Movement of Syrian Refugees over Turkish territory
            // Simulation mode Syria: Movement of Syrian IDPs over Syrian territory
            string simulationMode = "Turkey";

            string outputFileIdentifier = "";
            
            // the scenario consists of the model (represented by the model description)
            // and the simulation configuration (see config.json files)

            // Create a new model description that holds all parts of the model (agents, entities, layers)
            var description = new ModelDescription();

            description.AddLayer<ConflictLayer>();
            description.AddLayer<CampLayer>();
            description.AddLayer<PopulationLayer>();
            description.AddLayer<LocationLayer>();
            description.AddLayer<MigrantLayer>();
            description.AddLayer<SchedulerLayer>();
            description.AddAgent<MigrantAgent, MigrantLayer>();


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
            MigrantLayer migrantLayer = loopResults.Model.Layers.Values.OfType<MigrantLayer>().First();
            LocationLayer locationLayer = loopResults.Model.Layers.Values.OfType<LocationLayer>().First();
            Validation.NumSimRuns++;
            Validation.NumSteps = (int) loopResults.Iterations;
            Validation.FillRoutes(migrantLayer.RefugeeAgents);
            if(simulationMode.EqualsIgnoreCase("Syria"))  Validation.FillSyrianDistrictsPop(locationLayer.GetEntities().ToList());
            else Validation.FillTurkishDistrictsPop(locationLayer.GetEntities().ToList());
         

            if (MigrantAgent.Validate)
            {
                Validation.Print();
                for (int i = 0; i < 3; i++)
                {
                    task = SimulationStarter.Start(description, config);
                    loopResults = task.Run();
                    migrantLayer = loopResults.Model.Layers.Values.OfType<MigrantLayer>().First();
                    locationLayer = loopResults.Model.Layers.Values.OfType<LocationLayer>().First();
                    Validation.NumSimRuns++;
                    Validation.FillRoutes(migrantLayer.RefugeeAgents);
                    if(simulationMode.EqualsIgnoreCase("Syria"))  Validation.FillSyrianDistrictsPop(locationLayer.GetEntities().ToList());
                    else Validation.FillTurkishDistrictsPop(locationLayer.GetEntities().ToList());
                }
               
            }
            Validation.CalcAverageDistribution();
            if(simulationMode.EqualsIgnoreCase("Syria"))  Validation.WriteToFileSyria(outputFileIdentifier);
            else Validation.WriteToFileTurkey(outputFileIdentifier);


            // Feedback to user that simulation run was successful
            Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        }
    }
}