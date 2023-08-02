using LaserTagBox.Model.Refugee;
using Mars.Components.Environments;

namespace LaserTagBox.Model.Location.LocationNodes;

public class EnvironmentImpl : IGeoEnvironment
{
    public NodeLayer NodeLayer => NodeLayer.NodeLayerInstance;


    public GeoHashEnvironment<RefugeeAgent> GetEnvironment()
    {
       return  NodeLayer.GetEnvironment();
    }

    public ILocation GetLocationByName(string locationName)
    {
        return NodeLayer.GetLocationByName(locationName);
    }
}