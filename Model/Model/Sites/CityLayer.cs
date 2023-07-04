using System;
using System.Linq;
using Mars.Common;
using Mars.Components.Layers;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using ServiceStack;

namespace RefugeeSimulation.Model.Model.Sites;

public class CityLayer: VectorLayer<City>
{
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        Console.WriteLine(Entities.Count() + " Cities created!");
        return true;
    }
    
    public City GetNearestCity(Position pos)
    {
        var cityList = Entities.ToList();
        cityList.RemoveAll(city => city.GetCoordinate().ToPosition().Equals(pos));
       var nearestCity = cityList.OrderByDescending(city => pos.DistanceInKmTo(city.GetCoordinate().ToPosition())).FirstOrDefault();
       Console.WriteLine("Nearest City is " + nearestCity.GetName());
       return nearestCity;
    }

    public City getCityByName(String cityName)
    {
       var cityByName = Entities.ToList().Where(city => city.GetName().EqualsIgnoreCase(cityName));
       if (cityByName.Any())
       {
           return cityByName.First();
       }
       throw new ArgumentException("The city you input does cannot be found in the system.");
       
    }
}