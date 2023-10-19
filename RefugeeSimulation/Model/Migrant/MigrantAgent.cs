using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mars.Components.Environments;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Refugee;

public class MigrantAgent : IAgent<MigrantLayer>, IPositionable
{
    
  //------------------------------- Parameters needed to calculate location desirability ------------------------

    [PropertyDescription] public double MoveProbabilityConflict { get; set; }

    [PropertyDescription] public double MoveProbabilityCamp { get; set; }

    [PropertyDescription] public double MoveProbabilityOther { get; set; }


    [PropertyDescription] public double KinWeight { get; set; }

    [PropertyDescription] public double FriendWeight { get; set; }

    [PropertyDescription] public static int InitNumKins { get; set; }
    [PropertyDescription] public static int InitNumFriends { get; set; }
    
    private double _highestDesirabilityScore;
    private Location.Location _mostDesirable;
    
    
    
    // --------------------------------Layers--------------------------------------------------------------
    public MigrantLayer MigrantLayer { get; private set; }
   
    
    // ------------------------------------------- Agent information ---------------------------------
    
    public HashSet<MigrantAgent> Friends { get; set; }
    public HashSet<MigrantAgent> Kins { get; set; }
    public string LocationName { get; set; }

    public Location.Location Origin { get; set; }

    public Location.Location Current { get; set; }
    
    
    // Validation mode
    [PropertyDescription] public static bool Validate { get; set; }
    
    
    
    public GeoHashEnvironment<MigrantAgent> Environment;


    public void Init(MigrantLayer layer)
    {
        MigrantLayer = layer;
        Friends = new HashSet<MigrantAgent>();
        Kins = new HashSet<MigrantAgent>();
        Environment = MigrantLayer.Environment;
    }

    public void Tick()
    {
        var numCamps = Current.NumCamps;
        var numConflicts = Current.NumConflicts;

        if (!Activate(numCamps, numConflicts))
        {
            return;
        }


        _highestDesirabilityScore = 0;

        var neighbours = Current.Neighbors;


        if (neighbours.Count >= 1)
        {
            foreach (var n in neighbours)
            {
                Assess(n);
            }

            MoveToNode(_mostDesirable);
            
            // Collect Decision Making statistics
                Validation.NumDecisions++;
                switch (_mostDesirable.MigPop)
                {
                    case < 1000:
                        Validation.PopUnder1k++;
                        break;
                    case < 20000:
                        Validation.Pop1_20k++;
                        break;
                    case < 50000:
                        Validation.Pop20_50k++;
                        break;
                    default:
                        Validation.PopOver50k++;
                        break;
                }
                if (_mostDesirable.NumConflicts > 0)
                {
                    if (_mostDesirable.NumCamps > 0)
                    {
                        if (GetNumFriendsAtNode(_mostDesirable) > 0 ||
                            GetNumKinsAtNode(_mostDesirable) > 0)
                        {
                            Validation.HasAll++;
                        }
                        else
                        {
                            Validation.HasConflictAndCamp++;
                        }
                    }
                    else if (GetNumFriendsAtNode(_mostDesirable) > 0 ||
                             GetNumKinsAtNode(_mostDesirable) > 0)
                    {
                        Validation.HasConflictAndContacts++;
                    }
                    else
                    {
                        Validation.OnlyHasConflict++;
                    }
                }
                else if (GetNumFriendsAtNode(_mostDesirable) > 0 ||
                         GetNumKinsAtNode(_mostDesirable) > 0)
                {
                    if (_mostDesirable.NumCamps > 0)
                    {
                        Validation.HasCampAndContacts++;
                    }
                    else
                    {
                        Validation.OnlyHasContacts++;
                    }
                }
                else if (_mostDesirable.NumCamps > 0)
                {
                    Validation.OnlyHasCamp++;
                }
                else
                {
                    Validation.HasNone++;
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

        if (move) Validation.RefsActivated++;
        

        return move;
    }

    private void Assess(Location.Location node)
    {
        var nodeDesirability =
            CalcNodeDesirability(GetNumFriendsAtNode(node), GetNumKinsAtNode(node), node.Score);

        if (nodeDesirability > _highestDesirabilityScore)
        {
            _highestDesirabilityScore = nodeDesirability;
            _mostDesirable = node;
        }
    }

    private double CalcNodeDesirability(int numFriendsAtNode, int numKinsAtNode, double score)
    {
        return (numKinsAtNode * KinWeight) + (numFriendsAtNode * FriendWeight) + score;
    }

    public void MoveToNode(Location.Location @new)
    {
        Interlocked.Decrement(ref Current.MigPop);
        Current = @new;
        LocationName = @new.GetName();

        Environment.MoveToPosition(this, @new.Position.Latitude, @new.Position.Longitude);
        Interlocked.Increment(ref @new.MigPop);
    }

    public void InitSocialLinks()
    {
        var rand = new Random();
        for (int i = 0; i < InitNumKins; i++)
        {
            var nextKin = this;
            while (nextKin == this)
            {
                nextKin = MigrantLayer.RefugeeAgents[rand.Next(MigrantLayer.RefugeeAgents.Count)];
            }

            Kins.Add(nextKin);
            nextKin.Kins.Add(this);
        }

        for (int j = 0; j < InitNumFriends; j++)
        {
            var nextFriend = this;
            while (nextFriend == this)
            {
                nextFriend = MigrantLayer.RefugeeAgents[rand.Next(MigrantLayer.RefugeeAgents.Count)];
            }

            Kins.Add(nextFriend);
            nextFriend.Friends.Add(this);
        }
    }


    public int GetNumFriendsAtNode(Location.Location node)
    {

        var friendsAtNode = Friends.Where(agent => agent.LocationName.EqualsIgnoreCase(node.GetName())).ToList();


        return friendsAtNode.Count;
    }
    
    public int GetNumKinsAtNode(Location.Location node)
    {

        var kinsAtNode = Kins
            .Where(agent => agent.LocationName.EqualsIgnoreCase(node.GetName())).ToList();


        return kinsAtNode.Count;
    }

    public void FormFriendshipWith(MigrantAgent newFriend)
    {
        var other = newFriend;
        Friends.Add(other);
        other.Friends.Add(this);
    }

    public void Spawn(Location.Location node)
    {
        Origin = node;
        Current = node;
        LocationName = node.GetName();
        Interlocked.Increment(ref Current.MigPop);
        Position = Position.CreateGeoPosition(node.Position.Longitude,
            node.Position.Latitude);
        _mostDesirable = node;
    }


    public Guid ID { get; set; }
    public Position Position { get; set; }
}