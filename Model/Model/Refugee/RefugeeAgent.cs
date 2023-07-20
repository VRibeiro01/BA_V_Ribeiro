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
using IEnvironment = Mars.Interfaces.Environments.IEnvironment;
using Position = Mars.Interfaces.Environments.Position;

namespace RefugeeSimulation.Model.Model.Refugee;

public class RefugeeAgent : AbstractEnvironmentObject, IAgent<RefugeeLayer>
{
    
    
    public HashSet<RefugeeAgent> Friends { get; set; }
    public HashSet<RefugeeAgent> Kins { get; set; }
    public string LocationName { get; set; }
    
    public ILocation CurrentNode{ get; set; }

    private double HighestDesirabilityScore;
    private ILocation MostDesirableNode;

    // Layer
    public RefugeeLayer RefugeeLayer { get; private set; }
    public IGeoEnvironment IGeoEnvironment;

    
    // Parameters
    private double moveProbabilityCamp;

    private double moveProbabilityOther;

    private double KinWeight;
    private double FriendWeight;




    // Properties
    private static int _initNumKins;

    private static int _initNumFriends;
    
    
   
    
   
    
    public void Init(RefugeeLayer layer)
    {
        RefugeeLayer = layer;
        Friends = new HashSet<RefugeeAgent>();
        Kins = new HashSet<RefugeeAgent>();
        IGeoEnvironment = new IEnvironmentImpl();

    }
    
    public void Tick()
    {
        
        
        var numCamps = CurrentNode.GetNumCampsAtNode();
        var numConflicts = CurrentNode.GetNumConflictsAtNode();

        if (!Activate(numCamps, numConflicts))
        {
            return;
        }

        HighestDesirabilityScore = 0;

        var neighbours = CurrentNode.GetNeighbours();
        

        if (neighbours.Count >= 1)
        {

            foreach (var n in neighbours)
            {
                Assess(n, n.GetScore());
            }
            
            MoveToNode(MostDesirableNode);
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

    private void Assess(ILocation node, int score)
    {

        var nodeDesirability =
            CalcNodeDesirability(node, GetNumFriendsAtNode(node), GetNumKinsAtNode(node), node.GetScore());

        if (nodeDesirability > HighestDesirabilityScore)
        {
            HighestDesirabilityScore = nodeDesirability;
            MostDesirableNode = node;
        }
        
        
    }

    private double CalcNodeDesirability(ILocation node, int numFriendsAtNode, int numKinsAtNode, int score)
    {
        return ( (numKinsAtNode * KinWeight) + (numFriendsAtNode * FriendWeight) + score) ;
    }

    private void MoveToNode(ILocation newNode) {}

    private void InitSocialLinks(){}

    public void UpdateSocialNetwork(RefugeeAgent newFriend){}

    private int GetNumFriendsAtNode(ILocation node)
    {
      
        return 0;
    }

    private int GetNumKinsAtNode(ILocation node)
    {
        return 0;
    }

    public void Spawn(ILocation node)
    {
        CurrentNode = node;
        LocationName = node.GetName();
        Position = Position.CreateGeoPosition(node.GetCentroidPosition().Longitude,
            node.GetCentroidPosition().Latitude);
        MostDesirableNode = node;




        // TODO create social links
    }

    
    
}