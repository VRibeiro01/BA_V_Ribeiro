using Mars.Components.Environments;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using RefugeeSimulation.Model.Model.Shared;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public interface IEnvironment
{
    
    /// <summary>
    /// Returns the environment that contains refugees and locations
    /// </summary>
    /// <returns></returns>
    public GeoHashEnvironment<AbstractEnvironmentObject> GetEnvironment();

   /// <summary>
    /// Returns a reference to the location that corresponds to the name paremeter.
    /// If a location corresponding to the given name parameter isn't found, an ArgumentException will be thrown.
    /// </summary>
    /// <returns></returns>
    public ILocation GetLocationByName(string locationName);
   
}