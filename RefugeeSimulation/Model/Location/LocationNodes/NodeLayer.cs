using System;
using System.Linq;
using LaserTagBox.Model.Refugee;
using LaserTagBox.Model.Shared;
using Mars.Components.Environments;
using Mars.Components.Layers;
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

    public static Position AnchorCoordinates=  Position.CreateGeoPosition(AnchorLong, AnchorLat);// Lat= 41.015137, Long= 28.979530

    private GeoHashEnvironment<AbstractEnvironmentObject> Environment;







    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Environment = GeoHashEnvironment<AbstractEnvironmentObject>.BuildEnvironment(this.MaxLat, this.MinLat, this.MaxLon, this.MinLon, 100000);
        Debug();
        InsertLocationsInEnvironment();
        InitLocationParams();
        NodeLayerInstance = this;
        return true;
    }

    public GeoHashEnvironment<AbstractEnvironmentObject> GetEnvironment()
    {
        return Environment;
    }

    public void InsertLocationsInEnvironment()
    {
        foreach (var location in Entities)
        {
            Environment.Insert(location);
        }
    }

    public void InitLocationParams()
    {
        var maxNumCamps = Entities.Max(location => location.NumCamps) + 1; // add one to prevent division by zero
        var maxNumConflicts = Entities.Max(location => location.NumConflicts) +1;
        var maxAnchorScore = Entities.Max(location => location.AnchorScore);
        
        foreach (var locationNode in Entities)
        {
             locationNode.Neighbours.AddRange(Entities.Where(location =>
                location.GetCentroidPosition().DistanceInKmTo(locationNode.GetCentroidPosition()) > 5 &&
                location.GetCentroidPosition().DistanceInKmTo(locationNode.GetCentroidPosition()) <= 100  
            ).ToList());
             
             
             locationNode.NormNumCamps = locationNode.NumCamps * 1.0 / maxNumCamps;
             locationNode.NormNumConflicts = locationNode.NumConflicts  * 1.0/ maxNumConflicts;
             locationNode.NormAnchorScore = locationNode.AnchorScore * 1.0 / maxAnchorScore;
        }
    }

  
    

    public ILocation GetLocationByName(string locationName)
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
                    location.GetRandomRefugeesAtNode();
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

    private int MaxRefPop()
    {
        
        foreach (var location in Entities)
        {
           location.RefPop = Environment.Explore(location.GetCentroidPosition(), 0.01, -1, elem => elem is ISocialNetwork).Count();
            
        }

        return Entities.Max(location => location.RefPop);
    }

    

   
}