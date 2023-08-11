using System;
using System.Collections.Generic;
using System.Linq;
using LaserTagBox.Model.Location.LocationNodes;
using Mars.Components.Environments;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Refugee;

public class RefugeeAgent : IAgent<RefugeeLayer>, ISocialNetwork
{
    
    
    public HashSet<RefugeeAgent> Friends { get; set; }
    public HashSet<RefugeeAgent> Kins { get; set; }
    public string LocationName { get; set; }
    
    public LocationNode OriginNode { get; set; }
    
    public LocationNode CurrentNode{ get; set; }

    private double _highestDesirabilityScore;
    private LocationNode _mostDesirableNode;
    
    
    [PropertyDescription]
    public static bool Validate { get; set; }

    // Layer
    public RefugeeLayer RefugeeLayer { get; private set; }
    public GeoHashEnvironment<RefugeeAgent> Environment;

   
    
    
    
    
    
    
    // Parameter Properties
    
    [PropertyDescription]
    public double moveProbabilityCamp { get; set; }
    
    [PropertyDescription]
    public double moveProbabilityConflict { get; set; }

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
        Environment = RefugeeLayer.Environment;
        

    }
    
    public void Tick()
    {

        
        var numCamps = CurrentNode.GetNumCampsAtNode();
        var numConflicts = CurrentNode.GetNumConflictsAtNode();

        if (!Activate(numCamps, numConflicts))
        {
            return;
        }

        _highestDesirabilityScore = 0;

        var neighbours = CurrentNode.GetNeighbours();
        

        if (neighbours.Count >= 1)
        {

            foreach (var n in neighbours)
            {
                Assess(n);
            }
            
            MoveToNode(_mostDesirableNode);
            if (Validate)
            {
                Validation.NumDecisions++;
                if (_mostDesirableNode.GetNumConflictsAtNode() > 0)
                {
                    if (_mostDesirableNode.GetNumCampsAtNode() > 0)
                    {
                        if (GetNumFriendsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0 ||
                            GetNumKinsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0)
                        {
                            Validation.HasAll++;
                        }
                        else
                        {
                            Validation.HasConflictAndCamp++;
                        }
                    }
                    else if(GetNumFriendsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0 ||
                            GetNumKinsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0) 
                    {
                        Validation.HasConflictAndContacts++;
                    }
                    else
                    {
                        Validation.OnlyHasConflict++;
                    }
                    
                } else if (GetNumFriendsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0 ||
                           GetNumKinsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0)
                {
                    if (_mostDesirableNode.GetNumCampsAtNode() > 0)
                    {
                        Validation.HasCampAndContacts++;
                    }
                    else
                    {
                        Validation.OnlyHasContacts++;
                    }
                } else if (_mostDesirableNode.GetNumCampsAtNode() > 0)
                {
                    Validation.OnlyHasCamp++;
                }
                else
                {
                    Validation.HasNone++;
                }
   
            }
        }
        

    }

    private bool Activate(int numCamps, int numConflicts)
    {
        bool move;
        if (numConflicts > 0)
        {
            move = new Random().NextDouble() < moveProbabilityConflict;
        } else if (numCamps > 0)
        {
            move = new Random().NextDouble() < moveProbabilityCamp;
        }
        else
        {
            move = new Random().NextDouble() < moveProbabilityOther;
        }

        if (Validate && move)
        {
            Validation.RefsActivated++;
        }
        return move;
    }

    private void Assess(LocationNode node)
    {

        var agentList = RefugeeLayer.RefugeeAgents;
        var nodeDesirability =
            CalcNodeDesirability(GetNumFriendsAtNode(node, agentList), GetNumKinsAtNode(node, agentList), node.GetScore());

        if (nodeDesirability > _highestDesirabilityScore)
        {
            _highestDesirabilityScore = nodeDesirability;
            _mostDesirableNode = node;
        }
        
        
    }

    private double CalcNodeDesirability(int numFriendsAtNode, int numKinsAtNode, double score)
    {
        return ( (numKinsAtNode * KinWeight) + (numFriendsAtNode * FriendWeight) + score) ;
    }

    private void MoveToNode(LocationNode newNode)
    {
        CurrentNode.RefPop--;
        CurrentNode = newNode;
        LocationName = newNode.GetName();
        
        Environment.MoveToPosition(this, newNode.GetPosition().Latitude, newNode.GetPosition().Longitude);
        newNode.RefPop++;
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
            nextKin.Kins.Add(this);
        }

        for (int j = 0; j < _initNumFriends; j++)
        {
            var nextFriend = this;
            while (nextFriend == this)
            {
                nextFriend = RefugeeLayer.RefugeeAgents[rand.Next(RefugeeLayer.RefugeeAgents.Count)];
            }

            Kins.Add(nextFriend);
            nextFriend.Friends.Add(this);
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

    public void Spawn(LocationNode node)
    {
        OriginNode = node;
        CurrentNode = node;
        LocationName = node.GetName();
        CurrentNode.RefPop++;
        Position = Position.CreateGeoPosition(node.GetPosition().Longitude,
            node.GetPosition().Latitude);
        _mostDesirableNode = node;
        

    }


    public Guid ID { get; set; }
    public Position Position { get; set; }
}