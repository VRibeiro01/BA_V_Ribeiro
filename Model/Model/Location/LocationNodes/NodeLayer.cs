using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using ServiceStack;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public class NodeLayer : VectorLayer<LocationNode>, ISteppedActiveLayer
{
    
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Console.WriteLine(Entities.Count() + " Cities created!");
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
    
 
    public void Tick()
    {
        
    }

    public void PreTick()
    {
        
    }

    public void PostTick()
    {
        
    }
}