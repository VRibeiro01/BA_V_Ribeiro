using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using ServiceStack;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public class NodeLayer : VectorLayer<LocationNode>, ISteppedActiveLayer
{
    
 private double PopulationWeight { get; set; }

    private double CampWeight { get; set; }

    private double ConflictWeight { get; set; }

    private double LocationWeight{ get; set; }

    private Coordinate AnchorCoordinates { get; set; }
    
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        debug();
        return true;
    }
    
    public LocationNode GetCityByName(String cityName)
    {
        var cityByName = Entities.ToList().Where(city => city.GetName().EqualsIgnoreCase(cityName));
        if (cityByName.Any())
        {
            return cityByName.First();
        }
        throw new ArgumentException("The city you input does cannot be found in the system.");
       
    }

    private void debug()
    {
        Console.WriteLine(Entities.Count() + " Cities created!");
        var unknownCountries = Entities.ToList().Where(city => city.GetCountry().EqualsIgnoreCase("Unknown"));
        foreach (var c in unknownCountries)
        {
            Console.WriteLine(c.GetName());
        }
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
}