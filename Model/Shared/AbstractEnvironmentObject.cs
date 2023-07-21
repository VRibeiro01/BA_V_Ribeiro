using System;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace RefugeeSimulation.Model.Model.Shared;

public abstract class AbstractEnvironmentObject : IPositionable, IEntity
{
    public Position Position { get; set; }
    public Guid ID { get; set; }
}