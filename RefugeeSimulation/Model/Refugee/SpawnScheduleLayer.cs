using Mars.Interfaces;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Refugee;

public class SpawnScheduleLayer : ISteppedActiveLayer
{
    
    private ISimulationContext _simulationContext;
    
    [PropertyDescription]
    public RefugeeLayer RefugeeLayer { get; set; }
    
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
        RefugeeLayer.SpawnNewRefs();
    }

    public void PostTick()
    {
        
    }
}