using LaserTagBox.Model.Refugee;
using Mars.Components.Environments;

namespace LaserTagBox.Model.Location.LocationNodes;

public interface IGeoEnvironment
{
    
    /// <summary>
    /// Returns the environment that contains refugees and locations
    /// </summary>
    /// <returns></returns>
    public GeoHashEnvironment<ISocialNetwork> GetEnvironment();

   /// <summary>
    /// Returns a reference to the location that corresponds to the name paremeter.
    /// If a location corresponding to the given name parameter isn't found, an ArgumentException will be thrown.
    /// </summary>
    /// <returns></returns>
    public ILocation GetLocationByName(string locationName);
   
}