using System;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using RefugeeSimulation.Model.Model.Sites;

namespace RefugeeSimulation.Model.Model.Refugee;

public class SingleRefugeeGroup : IAgent<RefugeeLayer>
{
    
    // Group Attributes
    
    //---------------- Properties defined in input file ------------------------------------
    public City OriginCity;

    [PropertyDescription]
    public String OriginCityName { get; set; }
    
    [PropertyDescription]
    public int AdultMembers { get; set; }
    [PropertyDescription]
    public int ChildMembers { get; set; }
    
    [PropertyDescription]
    public int OldMembers { get; set; }
    
    [PropertyDescription]
    public int groupID { get; set; }

    //--------------------------------------------------------------------------------------------
    public AbstractSite LastVisitedSite;
    public int HungerLevel;
    public int HealthLevel;
    public int DeathToll;
    public Position Position;
    


    // Layers
    public RefugeeLayer RefugeeLayer { get; private set; }
    public CityLayer CityLayer { get; private set; }
    
    public ConflictLayer ConflictLayer { get; private set; }
    
    public void Init(RefugeeLayer layer)
    {
        RefugeeLayer = layer;
        OriginCity = CityLayer.getCityByName(OriginCityName);
        LastVisitedSite = OriginCity;
        Position = OriginCity.GetCoordinate().ToPosition();
        Console.WriteLine("Refugee agent created in " + OriginCityName + " with group ID " + groupID);

    }
    
    public void Tick()
    {
        Console.WriteLine("Current Location is " + LastVisitedSite.GetName());
       AbstractSite nextDestination = selectNextDestination();
       moveToNextDestination(nextDestination);


    }

    private void moveToNextDestination(AbstractSite nextDestination)
    {
        LastVisitedSite = nextDestination;
        Position = nextDestination.GetCoordinate().ToPosition();
    }

    private AbstractSite selectNextDestination()
    {
        City nearestCity = CityLayer.GetNearestCity(LastVisitedSite.GetCoordinate().ToPosition());
        if (!ConflictLayer.GetConflictStateForCity(nearestCity
                .GetName()))
        {
            Console.WriteLine("No conflict detected at nearest City");
            return nearestCity;
        }

        if (!(LastVisitedSite is City) || ((LastVisitedSite is City) &&
                                           !ConflictLayer.GetConflictStateForCity(LastVisitedSite.GetName())))
        {
            Console.WriteLine("Conflict at nearest city but current location is fine");
            return LastVisitedSite;    
        }

        return null;
    }
    
    public Guid ID { get; set; }
}