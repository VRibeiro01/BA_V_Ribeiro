using System;
using System.Collections.Generic;
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
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Location.LocationNodes;

public class NodeLayer : VectorLayer<LocationNode>, ISteppedActiveLayer
{
    public static NodeLayer NodeLayerInstance { get; private set; }

    public ISimulationContext SimulationContext;

    public int StartMonth;

    [PropertyDescription]                                       
    public double PopulationWeight { get; set; }

    [PropertyDescription]
    public double CampWeight { get; set; }

    [PropertyDescription]
    public double ConflictWeight { get; set; }

    [PropertyDescription]
    public double LocationWeight{ get; set; }

    [PropertyDescription]
    public static double AnchorLong { get; set; }
    
    [PropertyDescription]
    public static double AnchorLat { get; set; }
    
    [PropertyDescription]
    public int NumberNewTies { get; set; }

    public static Position AnchorCoordinates= Position.CreateGeoPosition(AnchorLong, AnchorLat);// Lat= 41.015137, Long= 28.979530

    private GeoHashEnvironment<RefugeeAgent> Environment;
    
    
    
    // Layers
    
    [PropertyDescription]
    public CampLayer CampLayer { get; set; }
    
    [PropertyDescription]
    public ConflictLayer ConflictLayer { get; set; }



    public String[] BorderCrossingNodes;





    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        SimulationContext = layerInitData.Context;
        if (StartMonth <= 0 && SimulationContext.StartTimePoint != null)
        {
            StartMonth = SimulationContext.StartTimePoint.Value.Month;
        }

        BorderCrossingNodes = new[]{"Lattakia", "Jisr-Ash-Shugur","Afrin","Al-Malikeyyeh", 
            "KILIS", "SANLIURFA", "MARDIN", "HATAY"};
        

        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Environment = GeoHashEnvironment<RefugeeAgent>.BuildEnvironment(this.MaxLat, this.MinLat, this.MaxLon, this.MinLon);
        Debug();
        InitLocationParams();
        NodeLayerInstance = this;
        return true;
    }

    public GeoHashEnvironment<RefugeeAgent> GetEnvironment()
    {
        return Environment;
    }

  
    public void InitLocationParams()
    {
        var maxNumCamps = Entities.Max(location => location.NumCamps) + 1; // add one to prevent division by zero
        var maxNumConflicts = Entities.Max(location => location.NumConflicts) +1;
        var maxAnchorScore = Entities.Max(location => location.AnchorScore);
        
        foreach (var locationNode in Entities)
        {
             locationNode.Neighbours.AddRange(Entities.Where(location =>
                location != locationNode &&
                location.GetPosition().DistanceInKmTo(locationNode.GetPosition()) <= 25
            ).ToList());
             
             
             
             
             locationNode.NormNumCamps = locationNode.NumCamps * 1.0 / (maxNumCamps * 1.0);
             locationNode.NormNumConflicts = locationNode.NumConflicts  * 1.0/ (maxNumConflicts * 1.0);
             locationNode.NormAnchorScore = locationNode.AnchorScore * 1.0 / (maxAnchorScore * 1.0 );
        }
    }

  
    

    public LocationNode GetLocationByName(string locationName)
    {
        var locationByName = Entities.ToList().Where(city =>
            city.GetName().Trim().EqualsIgnoreCase(locationName.Trim()));

        // put collected results in a list
        var locationNodes = locationByName.ToList();
        
        // return result
        if (locationNodes.Any())
        {
            return locationNodes.First();
        }

        // error handling
        throw new ArgumentException("The location "+ locationName + "  cannot be found in the system.");
       
    }

    private void Debug()
    {
        if (!Entities.Any()) throw new ArgumentException("No Entities in Nodelayer");
        
        Console.WriteLine(Entities.Count() + " Cities created!");
        var unknownCountries = Entities.ToList().Where(city => city.GetCountry().EqualsIgnoreCase("Unknown"));
        foreach (var c in unknownCountries)
        {
            Console.WriteLine(c.GetName());
        }
        
        Console.WriteLine("Conflict Weight Data Type Test:  " + (ConflictWeight + LocationWeight));
    }
    
 
    public void Tick()
    {
        
    }

    public void PreTick()
    {
        var maxRefPop = MaxRefPop();
        foreach (var location in Entities)
        {
            location.UpdateNormRefPop(maxRefPop);
             CalcScore(location);
        }
    }

    public void PostTick()
    {
        foreach (var location in Entities)
        {
            if (location.NumCamps > 0)
            {
                for (int i = 0; i < NumberNewTies; i++)
                {
                    location.GetRandomRefugeesAtNode(Environment);
                }
            }
        }
     
    }


    private void CalcScore(LocationNode location)
    {
        
            location.Score = (location.NormRefPop * PopulationWeight) + (location.NormAnchorScore * LocationWeight)
                                                                      + (location.NormNumCamps * CampWeight) +
                                                                      (location.NormNumConflicts * (-1) *
                                                                       ConflictWeight);
        
    }

    public int MaxRefPop()
    {
        
        foreach (var location in Entities)
        {
           location.RefPop = Environment.Explore(location.Position, -1D, -1, elem => 
               location.Position.DistanceInKmTo(elem.Position) < 1).Count();
            
        }

        return Entities.Max(location => location.RefPop);
    }

    public List<LocationNode> GetEntities()
    {
        return Entities.ToList();
    }

    

   
}