using System;
using System.Collections.Generic;
using LaserTagBox.Model.Location.LocationNodes;
using Mars.Interfaces;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using ServiceStack;

namespace LaserTagBox.Model.Refugee.Scheduler;

public class SchedulerLayer : ISteppedActiveLayer
{
    
    
    private ISimulationContext _simulationContext;

    [PropertyDescription] 
    public MigrantLayer MigrantLayer { get; set; }
    
    
    [PropertyDescription]
    public string Mode { get; set; }

    private int months = 1;

    public bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        _simulationContext = layerInitData.Context;
        return true;
    }

    public long GetCurrentTick()
    {
        return _simulationContext.CurrentTick;
    }

    public void SetCurrentTick(long currentStep)
    {
    }

    public void Tick()
    {
    }

    public void PreTick()
    {
        if (Mode.EqualsIgnoreCase("Syria"))
        {
            MigrantLayer.SpawnNewIDPs();
        }
        else
        {
            MigrantLayer.SpawnNewRefs();
        }
    }

    public void PostTick()
    {
    }
}