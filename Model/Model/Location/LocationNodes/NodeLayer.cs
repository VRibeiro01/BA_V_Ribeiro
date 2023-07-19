using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using RefugeeSimulation.Model.Model.Refugee;
using RefugeeSimulation.Model.Model.Shared;
using ServiceStack;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public class NodeLayer : VectorLayer<LocationNode>, ISteppedActiveLayer
{
    
    [PropertyDescription]                                       
    public double PopulationWeight { get; set; }

    [PropertyDescription]
    public double CampWeight { get; set; }

    [PropertyDescription]
    public double ConflictWeight { get; set; }

    [PropertyDescription]
    public double LocationWeight{ get; set; }

    [PropertyDescription]
    public double AnchorLong { get; set; }
    
    [PropertyDescription]
    public double AnchorLat { get; set; }
    
    private Coordinate AnchorCoordinates { get; set; } // Lat= 41.015137, Long= 28.979530

    private List<LocationNode> EntitiesList;

    

    

    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Debug();
        return true;
    }

    public GeoHashEnvironment<AbstractEnvironmentObject> GetEnvironment()
    {
        return null;
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
        
    }

    public void PostTick()
    {
     
    }


    private double CalcScores()
    {
        return 0.0;
    }

    private int MaxRefPop()
    {
        return 0;
    }
}