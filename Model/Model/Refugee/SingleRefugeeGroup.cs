using System;
using System.Collections.Specialized;
using System.Linq;
using LaserTagBox.Model.Model.Location;
using LaserTagBox.Model.Model.Location.Camps;
using LaserTagBox.Model.Model.Location.LocationNodes;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Numerics;
using Microsoft.CodeAnalysis.Text;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace RefugeeSimulation.Model.Model.Refugee;

public class SingleRefugeeGroup : IAgent<RefugeeLayer>
{
    
    // Group Attributes
    
    //---------------- Properties defined in input file ------------------------------------
    public LocationNode OriginCity;

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
    public NodeLayer NodeLayer { get; private set; }
    
   
    
    public CampLayer CampLayer { get; private set; }
    
    public void Init(RefugeeLayer layer)
    {
        RefugeeLayer = layer;
        OriginCity = NodeLayer.GetCityByName(OriginCityName);
        Position = OriginCity.GetCentroidPosition();
        Console.WriteLine("Refugee agent created in " + OriginCityName + " with group ID " + groupID);

    }
    
    public void Tick()
    {
        /*Console.WriteLine("Current Location is " + LastVisitedSite.GetName());
       AbstractSite nextDestination = selectNextDestination();
       moveToNextDestination(nextDestination);*/
        //Console.WriteLine(OriginCity.GetName() + " 's Coordinate: " + OriginCity.GetCoordinate());
        //var c = OriginCity.GetCoordinates();
        //var cams = CampLayer.GetCampsAroundPosition(Position, 20);
       //Console.WriteLine("No of Residents: " + OriginCity.GetResidents());

        /*for (int x = 0; x < cams.Count; x++)
        {
            var camp = cams[x];
            if (camp.GetCoordinates().IsWithinDistance(c,0))
            {
                Console.WriteLine("Camp " + x + " in Al-Hole" + "\n" + "Camp coordinates: " + camp.GetCoordinate());
            }
        }
        
        OriginCity.EnterCity();*/
        
        
     


    }

    /*private void moveToNextDestination(AbstractSite nextDestination)
    {
        LastVisitedSite = nextDestination;
        Position = nextDestination.GetCoordinate().ToPosition();
    }*/

    /*private AbstractSite selectNextDestination()
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
    }*/
    
    public Guid ID { get; set; }
}