using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.Camps;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Refugee;
using Mars.Common.Core.Collections;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Interfaces;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Location.LocationNodes;

public class NodeLayer : VectorLayer<LocationNode>, ISteppedActiveLayer
{

    //------------------------------- Parameters needed to calculate location scores ------------------------
    [PropertyDescription] public double PopulationWeight { get; set; }

    [PropertyDescription] public double CampWeight { get; set; }

    [PropertyDescription] public double ConflictWeight { get; set; }

    [PropertyDescription] public double LocationWeight { get; set; }

    [PropertyDescription] public static double AnchorLong { get; set; }

    [PropertyDescription] public static double AnchorLat { get; set; }

    [PropertyDescription] public int NumberNewTiesUpper { get; set; }

    [PropertyDescription] public int NumberNewTiesLower { get; set; }

    public static Position
        AnchorCoordinates = Position.CreateGeoPosition(AnchorLong, AnchorLat); 

    


    // -----------------------------------------Layers -------------------------------------

    [PropertyDescription] public CampLayer CampLayer { get; set; }

    [PropertyDescription] public ConflictLayer ConflictLayer { get; set; }
    

 //-----------------------------------------Environments------------------------------------------------------
    private GeoHashEnvironment<RefugeeAgent> _refugeeEnvironment;
    
    
    // ---------------------------------------------------------------------------------------------------------
    
    public List<String> BorderCrossingNodes;
    public List<LocationNode> EntitiesList { get; set; }
    public ISimulationContext SimulationContext;
    public int StartMonth;
    public int EndMonth;
    
    //---------------------------------------Variables Syria IDP simulation----------------------------------------
    [PropertyDescription]
    public string Mode { get; set; }
    
    public double MaxSpawnFactor { get; set; }
    
    [PropertyDescription] 
    public PopulationLayer PopulationLayer { get; set; }
    
    // ----------------------------------------------------------------------------------------------

    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgentHandle = null)
    {
        SimulationContext = layerInitData.Context;
        
        
        if (StartMonth <= 0 && SimulationContext.StartTimePoint != null)
        {
            StartMonth = SimulationContext.StartTimePoint.Value.Month;
        }
        if (EndMonth <= 0 && SimulationContext.EndTimePoint != null)
        {
            EndMonth = SimulationContext.EndTimePoint.Value.Month;
        }

        BorderCrossingNodes = new();
        InitBorderCrossingsFromFile("Turkey");
        
        
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
       
        
        _refugeeEnvironment =
            GeoHashEnvironment<RefugeeAgent>.BuildEnvironment(this.MaxLat, this.MinLat, this.MaxLon, this.MinLon);
        EntitiesList = Entities.ToList();
        RemoveDupes();
        Debug();
        PopulationLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine(
                    "C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\syrPop_adm3.csv")
            }
        });
        InitLocationParams();
        return true;
    }

    public DateTime GetCurrentTimePoint()
    {
        if (SimulationContext.CurrentTimePoint is null)
        {
            throw new NoNullAllowedException("Simulation is not time based. Access to time point not possible");
        }
        return (DateTime) SimulationContext.CurrentTimePoint;
    }
    private void RemoveDupes()
    {
        var list = new List<String>();
        
        foreach (var locationNode in Entities)
        {
            if (!list.Contains(locationNode.GetName()))
            {
                list.Add(locationNode.GetName());
            }
            else
            {
                // Different district same english name spelling in SyriaADM3
                if(locationNode.GetName().EqualsIgnoreCase("Suran") && 
                   locationNode.GetProvinceName().EqualsIgnoreCase("Hama"))
                {
                    locationNode.VectorStructured.Data["Name3"] = "Sawran";
                }
                else
                {
                    EntitiesList.Remove(locationNode);
                }
            }
        }
    }

    public GeoHashEnvironment<RefugeeAgent> GetEnvironment()
    {
        return _refugeeEnvironment;
    }
   

    public void InitLocationParams()
    {
        var maxNumCamps = EntitiesList.Max(location => location.NumCamps) + 1; // add one to prevent division by zero
        var maxNumConflicts = EntitiesList.Max(location => location.NumConflicts) + 1;
        var maxAnchorScore = EntitiesList.Max(location => location.AnchorScore);

       
            double maxPop = 1.0;
            if (Mode.EqualsIgnoreCase("Syria"))
            {
                foreach (var locationPopPair in PopulationLayer.SyriaPopulationData)
                {
                    GetLocationByName(locationPopPair.Key).Population = locationPopPair.Value;
                }
                maxPop = EntitiesList.Max(l => l.Population) + 1.0;
            }

            
            
            
        

        foreach (var locationNode in EntitiesList)
        {
            

            locationNode.Neighbours.AddRange(EntitiesList.Where(location =>
                location != locationNode &&
                location.Position.DistanceInKmTo(locationNode.Position) <= 200
            ).OrderBy(n => locationNode.Position.DistanceInKmTo(n.Position))
                .Take(4));
            
         


            locationNode.NormNumCamps = locationNode.NumCamps * 1.0 / (maxNumCamps * 1.0);
            locationNode.NormNumConflicts = locationNode.NumConflicts * 1.0 / (maxNumConflicts * 1.0);
            locationNode.NormAnchorScore = locationNode.AnchorScore * 1.0 / (maxAnchorScore * 1.0);
            if (Mode.EqualsIgnoreCase("Syria"))
            {
                locationNode.NormPop = locationNode.Population * 1.0 / maxPop;
            }
            
        }
    }


    public LocationNode GetLocationByName(string locationName)
    {
        var locationByName = EntitiesList.Where(city =>
            city.GetName().Trim().EqualsIgnoreCase(locationName.Trim()));

        // put collected results in a list
        var locationNodes = locationByName.ToList();

        // return result
        if (locationNodes.Any())
        {
            return locationNodes.First();
        }

        // error handling
        throw new ArgumentException("The location " + locationName + "  cannot be found in the system.");
    }

  
    public List<LocationNode> GetLocationsInProvince(string provinceName)
    {
        var locationsInProvince = EntitiesList.Where(prov =>
            prov.GetProvinceName().Trim().EqualsIgnoreCase(provinceName.Trim()));

        // put collected results in a list
        var locationNodes = locationsInProvince.ToList();

        // return result
        if (locationNodes.Any())
        {
            return locationNodes;
        }

        // error handling
        throw new ArgumentException("The province " + provinceName + "  cannot be found in the system.");
    }

    private void Debug()
    {
        if (!EntitiesList.Any()) throw new ArgumentException("No Entities in Nodelayer");

        Console.WriteLine(EntitiesList.Count + " Cities created!");
        var unknownCountries = EntitiesList.Where(city => city.Country.EqualsIgnoreCase("Unknown"));
        foreach (var c in unknownCountries)
        {
            Console.WriteLine(c.GetName());
        }
        
        /*BorderCrossingNodes.Select(
            i
                => string.Join(",", i)).ToList().ForEach(Console.WriteLine);*/
    }


    public void Tick()
    {
       
    }

    public void PreTick()
    {
        if (Mode.EqualsIgnoreCase("Syria"))
        {
            foreach (var location in EntitiesList)
            {
                location.InitConflicts(ConflictLayer);
            }
        }
    }

    public void UpdateNodeScores()
    {
        /*if (Mode.EqualsIgnoreCase("Syria"))
        {
            foreach (var location in EntitiesList)
            {
                CalcScore(location);
            }

            return;
        }*/
        var maxRefPop = MaxRefPop();
        foreach (var location in EntitiesList)
        {
            location.UpdateNormRefPop(maxRefPop);
            CalcScore(location);
        }
    }

    public void PostTick()
    {

        var random = new Random();
        foreach (var location in EntitiesList)
        {
            if (location.NumCamps > 0)
            {
                var bound = random.Next(NumberNewTiesLower, NumberNewTiesUpper + 1);
                for (int i = 0; i < bound; i++)
                {
                    location.GetRandomRefugeesAtNode(_refugeeEnvironment);
                }
            }
        }

        if (RefugeeAgent.Validate)
        {
            Validation.IncrementPercentageActivatedRefs();
        }
        
        UpdateNodeScores();
    }


    private void CalcScore(LocationNode location)
    {
        /*if (Mode.EqualsIgnoreCase("Syria"))
        {
            location.Score = (location.NormPop * PopulationWeight) + (location.NormAnchorScore * LocationWeight)
                                                                      + (location.NormNumCamps * CampWeight) +
                                                                      (location.NormNumConflicts * (-1) *
                                                                       ConflictWeight);
            return;
        }*/
        
        location.Score = (location.NormRefPop * PopulationWeight) + (location.NormAnchorScore * LocationWeight)
                                                                  + (location.NormNumCamps * CampWeight) +
                                                                  (location.NormNumConflicts * (-1) *
                                                                   ConflictWeight);
    }

    public int MaxRefPop()
    {
        return EntitiesList.Max(location => location.RefPop);
    }

    public List<LocationNode> GetEntities()
    {
        return EntitiesList;
    }

    private void InitBorderCrossingsFromFile(string country)
    {
        string basepath = Path.Combine(Environment.CurrentDirectory, @"Resources");

        Path.Combine(Environment.CurrentDirectory, @"Resources");
        string[] lines = System.IO.File.ReadAllLines(Path.Combine(basepath, "border_crossing_points.csv"));
        foreach (string line in lines)
        {
            string[] columns = line.Split(',');
            if (columns[1].EqualsIgnoreCase(country))
            {
                BorderCrossingNodes.Add(columns[0]);
            }
        }
    }
}