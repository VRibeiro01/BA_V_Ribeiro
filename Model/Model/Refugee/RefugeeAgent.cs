using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using LaserTagBox.Model.Model.Location;
using LaserTagBox.Model.Model.Location.Camps;
using LaserTagBox.Model.Model.Location.LocationNodes;
using Mars.Common;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Numerics;
using Microsoft.CodeAnalysis.Text;
using MongoDB.Driver.Core.Operations;
using NetTopologySuite.Geometries;
using RefugeeSimulation.Model.Model.Shared;
using Position = Mars.Interfaces.Environments.Position;

namespace RefugeeSimulation.Model.Model.Refugee;

public class RefugeeAgent : AbstractEnvironmentObject, IAgent<RefugeeLayer>
{
    
    
    public HashSet<RefugeeAgent> Friends { get; set; }
    public HashSet<RefugeeAgent> Kins { get; set; }
    public string LocationName { get; set; }
    
    public ILocation CurrentNode{ get; set; }


    // Layer
    public RefugeeLayer RefugeeLayer { get; private set; }

    
    // Parameters
    private double moveProbabilityCamp;

    private double moveProbabilityOther;




    // Properties
    private static int _initNumKins;

    private static int _initNumFriends;
    
    
   
    
   
    
    public void Init(RefugeeLayer layer)
    {
        RefugeeLayer = layer;
        Friends = new HashSet<RefugeeAgent>();
        Kins = new HashSet<RefugeeAgent>();

    }
    
    public void Tick()
    {

        var numCamps = CurrentNode.GetNumCampsAtNode();
        var numConflicts = CurrentNode.GetNumConflictsAtNode();

        if (!Activate(numCamps, numConflicts))
        {
            return;
        }
      
        
        

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
        
     */





    }

    private bool Activate(int numCamps, int numConflicts)
    {
        var move = false;
        if (numCamps > 0)
        {
            move = new Random().NextDouble() < moveProbabilityCamp;
        } else if (numConflicts > 0)
        {
            move = true;
        }
        else
        {
            move = new Random().NextDouble() < moveProbabilityOther;
        }
        return false;
    }

    private ILocation Assess()
    {
        return CurrentNode;
    }

    private double CalcNodeDesirability(LocationNode node, int numFriendsAtNode, int numKinsAtNode)
    {
        return 0.0;
    }

    private void MoveToNode(LocationNode newNode) {}

    private void InitSocialLinks(){}

    public void UpdateSocialNetwork(RefugeeAgent newFriend){}

    public void Spawn(ILocation node)
    {
        CurrentNode = node;
        LocationName = node.GetName();
        Position = Position.CreateGeoPosition(node.GetCentroidPosition().Longitude,
            node.GetCentroidPosition().Latitude);
        
        
       
        
        // TODO create social links
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
    
    
    
}