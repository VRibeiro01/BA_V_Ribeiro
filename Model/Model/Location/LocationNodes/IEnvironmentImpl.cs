using Mars.Components.Environments;
using RefugeeSimulation.Model.Model.Shared;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public class IEnvironmentImpl : IGeoEnvironment
{
    public NodeLayer NodeLayer => NodeLayer.NodeLayerInstance;


    public GeoHashEnvironment<AbstractEnvironmentObject> GetEnvironment()
    {
       return  NodeLayer.GetEnvironment();
    }

    public ILocation GetLocationByName(string locationName)
    {
        return NodeLayer.GetLocationByName(locationName);
    }
}