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

public class RefugeeAgent : IAgent<RefugeeLayer>, IPositionable
{
    public HashSet<RefugeeAgent> Friends { get; set; }
    public HashSet<RefugeeAgent> Kins { get; set; }
    public string LocationName { get; set; }

    public LocationNode OriginNode { get; set; }

    public LocationNode CurrentNode { get; set; }

    private double _highestDesirabilityScore;
    private LocationNode _mostDesirableNode;


    [PropertyDescription] public static bool Validate { get; set; }

    // Layer
    public RefugeeLayer RefugeeLayer { get; private set; }
    public GeoHashEnvironment<RefugeeAgent> Environment;


    // Parameter Properties

    [PropertyDescription] public double MoveProbabilityConflict { get; set; }

    [PropertyDescription] public double MoveProbabilityCamp { get; set; }

    [PropertyDescription] public double MoveProbabilityOther { get; set; }


    [PropertyDescription] public double KinWeight { get; set; }

    [PropertyDescription] public double FriendWeight { get; set; }

    [PropertyDescription] public static int InitNumKins { get; set; }
    [PropertyDescription] public static int InitNumFriends { get; set; }


    public void Init(RefugeeLayer layer)
    {
        RefugeeLayer = layer;
        Friends = new HashSet<RefugeeAgent>();
        Kins = new HashSet<RefugeeAgent>();
        Environment = RefugeeLayer.Environment;
    }

    public void Tick()
    {
        var numCamps = CurrentNode.NumCamps;
        var numConflicts = CurrentNode.NumConflicts;

        if (!Activate(numCamps, numConflicts))
        {
            return;
        }


        _highestDesirabilityScore = 0;

        var neighbours = CurrentNode.Neighbours;


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
                if (_mostDesirableNode.NumConflicts > 0)
                {
                    if (_mostDesirableNode.NumCamps > 0)
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
                    else if (GetNumFriendsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0 ||
                             GetNumKinsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0)
                    {
                        Validation.HasConflictAndContacts++;
                    }
                    else
                    {
                        Validation.OnlyHasConflict++;
                    }
                }
                else if (GetNumFriendsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0 ||
                         GetNumKinsAtNode(_mostDesirableNode, RefugeeLayer.RefugeeAgents) > 0)
                {
                    if (_mostDesirableNode.NumCamps > 0)
                    {
                        Validation.HasCampAndContacts++;
                    }
                    else
                    {
                        Validation.OnlyHasContacts++;
                    }
                }
                else if (_mostDesirableNode.NumCamps > 0)
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
        var random = new Random();
        if (numConflicts > 0)
        {
            move = random.NextDouble() < MoveProbabilityConflict;
        }
        else if (numCamps > 0)
        {
            move = random.NextDouble() < MoveProbabilityCamp;
        }
        else
        {
            move = random.NextDouble() < MoveProbabilityOther;
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
            CalcNodeDesirability(GetNumFriendsAtNode(node, agentList), GetNumKinsAtNode(node, agentList), node.Score);

        if (nodeDesirability > _highestDesirabilityScore)
        {
            _highestDesirabilityScore = nodeDesirability;
            _mostDesirableNode = node;
        }
    }

    private double CalcNodeDesirability(int numFriendsAtNode, int numKinsAtNode, double score)
    {
        return ((numKinsAtNode * KinWeight) + (numFriendsAtNode * FriendWeight) + score);
    }

    private void MoveToNode(LocationNode newNode)
    {
        CurrentNode.RefPop--;
        CurrentNode = newNode;
        LocationName = newNode.GetName();

        Environment.MoveToPosition(this, newNode.Position.Latitude, newNode.Position.Longitude);
        newNode.RefPop++;
    }

    public void InitSocialLinks()
    {
        var rand = new Random();
        for (int i = 0; i < InitNumKins; i++)
        {
            var nextKin = this;
            while (nextKin == this)
            {
                nextKin = RefugeeLayer.RefugeeAgents[rand.Next(RefugeeLayer.RefugeeAgents.Count)];
            }

            Kins.Add(nextKin);
            nextKin.Kins.Add(this);
        }

        for (int j = 0; j < InitNumFriends; j++)
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


    public int GetNumFriendsAtNode(LocationNode node, List<RefugeeAgent> agentList)
    {
        var agentsInRadius = GetAgentsAtNode(node, agentList);

        var friendsAtNode = agentsInRadius.Where(agent => Friends.Contains(agent)).ToList();


        return friendsAtNode.Count;
    }

    private List<RefugeeAgent> GetAgentsAtNode(LocationNode node, List<RefugeeAgent> agentList)
    {
        var agentsInRadius = agentList
            .Where(a => a.LocationName.EqualsIgnoreCase(node.GetName())).ToList();


        return agentsInRadius;
    }

    public int GetNumKinsAtNode(LocationNode node, List<RefugeeAgent> agentList)
    {
        var agentsInRadius = GetAgentsAtNode(node, agentList);

        var kinsAtNode = agentsInRadius
            .Where(agent => Kins.Contains(agent)).ToList();


        return kinsAtNode.Count;
    }

    public void UpdateSocialNetwork(RefugeeAgent newFriend)
    {
        var other = newFriend;
        Friends.Add(other);
        other.Friends.Add(this);
    }

    public void Spawn(LocationNode node)
    {
        OriginNode = node;
        CurrentNode = node;
        LocationName = node.GetName();
        CurrentNode.RefPop++;
        Position = Position.CreateGeoPosition(node.Position.Longitude,
            node.Position.Latitude);
        _mostDesirableNode = node;
    }


    public Guid ID { get; set; }
    public Position Position { get; set; }
}