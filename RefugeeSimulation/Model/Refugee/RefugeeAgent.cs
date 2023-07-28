using System;
using System.Collections.Generic;
using System.Linq;
using LaserTagBox.Model.Location.LocationNodes;
using Mars.Components.Environments;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Refugee;

public class RefugeeAgent : IAgent<RefugeeLayer>, ISocialNetwork
{
    
    
    public HashSet<RefugeeAgent> Friends { get; set; }
    public HashSet<RefugeeAgent> Kins { get; set; }
    public string LocationName { get; set; }
    
    public ILocation OriginNode { get; set; }
    
    public ILocation CurrentNode{ get; set; }

    private double HighestDesirabilityScore;
    private ILocation MostDesirableNode;

    // Layer
    public RefugeeLayer RefugeeLayer { get; private set; }
    public GeoHashEnvironment<ISocialNetwork> Environment;

    
    
    
    
    
    
    // Parameter Properties
    
    [PropertyDescription]
    public double moveProbabilityCamp { get; set; }

    [PropertyDescription]
    public double moveProbabilityOther { get; set; }

    [PropertyDescription]
    public double KinWeight { get; set; }
    
    [PropertyDescription]
    public double FriendWeight { get; set; }
    
    [PropertyDescription]
    public static int _initNumKins { get; set; }
    [PropertyDescription]
    public static int _initNumFriends { get; set; }
    
    
   
    
   
    
    public void Init(RefugeeLayer layer)
    {
        RefugeeLayer = layer;
        Friends = new HashSet<RefugeeAgent>();
        Kins = new HashSet<RefugeeAgent>();
        Environment = new EnvironmentImpl().GetEnvironment();

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
                Assess(n);
            }
            
            MoveToNode(MostDesirableNode);
        }
       



        /*Console.WriteLine("Current Location is " + LastVisitedSite.GetName());
       AbstractSite nextDestination = selectNextDestination();
       moveToNextDestination(nextDestination);*/
        //Console.WriteLine(OriginCity.GetName() + " 's Coordinate: " + OriginCity.GetCoordinate());
        //var c = OriginCity.GetConflictGeometry();
        //var cams = CampLayer.GetCampsAroundPosition(Position, 20);
        //Console.WriteLine("No of Residents: " + OriginCity.GetResidents());

        /*for (int x = 0; x < cams.Count; x++)
        {
            var camp = cams[x];
            if (camp.GetConflictGeometry().IsWithinDistance(c,0))
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
        return move;
    }

    private void Assess(ILocation node)
    {

        var agentList = RefugeeLayer.RefugeeAgents;
        var nodeDesirability =
            CalcNodeDesirability(GetNumFriendsAtNode(node, agentList), GetNumKinsAtNode(node, agentList), node.GetScore());

        if (nodeDesirability > HighestDesirabilityScore)
        {
            HighestDesirabilityScore = nodeDesirability;
            MostDesirableNode = node;
        }
        
        
    }

    private double CalcNodeDesirability(int numFriendsAtNode, int numKinsAtNode, double score)
    {
        return ( (numKinsAtNode * KinWeight) + (numFriendsAtNode * FriendWeight) + score) ;
    }

    private void MoveToNode(ILocation newNode)
    {
        CurrentNode = newNode;
        LocationName = newNode.GetName();
        
        Environment.MoveToPosition(this, newNode.GetPosition().Latitude, newNode.GetPosition().Longitude);
    }
    
    public void InitSocialLinks()
    {
        var rand = new Random();
        for (int i = 0; i < _initNumKins; i++)
        {
            var nextKin = this;
            while (nextKin == this)
            {
                nextKin = RefugeeLayer.RefugeeAgents[rand.Next(RefugeeLayer.RefugeeAgents.Count)];
            }

            Kins.Add(nextKin);
        }

        for (int j = 0; j < _initNumFriends; j++)
        {
            var nextFriend = this;
            while (nextFriend == this)
            {
                nextFriend = RefugeeLayer.RefugeeAgents[rand.Next(RefugeeLayer.RefugeeAgents.Count)];
            }

            Kins.Add(nextFriend);
        }
    }

    

    public int GetNumFriendsAtNode(ILocation node, List<RefugeeAgent> agentList)
    {
        var agentsInRadius = GetAgentsAtNode(node, agentList);
        
       var friendsAtNode = agentsInRadius.Where(agent => Friends.Contains(agent)).ToList();


       
        return friendsAtNode.Count;
    }

    private List<RefugeeAgent> GetAgentsAtNode(ILocation node, List<RefugeeAgent> agentList)
    {
        var agentsInRadius = agentList
            .Where(a => a.LocationName.EqualsIgnoreCase(node.GetName())).ToList();
        
        
        return agentsInRadius;
    }

    public int GetNumKinsAtNode(ILocation node, List<RefugeeAgent> agentList)
    {
        var agentsInRadius = GetAgentsAtNode(node, agentList);
        
        var kinsAtNode = agentsInRadius
            .Where(agent => Kins.Contains(agent)).ToList();


       
        return kinsAtNode.Count;
    }

    public void UpdateSocialNetwork(ISocialNetwork newFriend)
    {
        var other = (RefugeeAgent) newFriend;
        Friends.Add(other);
        other.Friends.Add(this);
    }

    public void Spawn(ILocation node)
    {
        OriginNode = node;
        CurrentNode = node;
        LocationName = node.GetName();
        Position = Position.CreateGeoPosition(node.GetPosition().Longitude,
            node.GetPosition().Latitude);
        MostDesirableNode = node;
        

    }


    public Guid ID { get; set; }
    public Position Position { get; set; }
}