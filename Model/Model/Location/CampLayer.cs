using System;
using System.Linq;
using Mars.Common;
using Mars.Common.IO.Mapped.Collections;
using Mars.Components.Layers;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Model.Location;

public class CampLayer:VectorLayer<Camp>
{
    
    [PropertyDescription]
    public List<String> ExcludedCountries { get; set; }

    
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null, UnregisterAgent unregisterAgentHandle = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        
        // Debug output
        
       Console.WriteLine(Entities.Count() + " Camps created!");
       Console.WriteLine(string.Format("Excluded Camps in: ({0}).", string.Join(", ", ExcludedCountries)));
        return true;
    }
    
    public System.Collections.Generic.List<Camp> GetCampsAroundPosition(Mars.Interfaces.Environments.Position pos, double radiusKM)
    {
       
        return this.Entities.ToList().Where(elem => pos.DistanceInKmTo(elem.GetCoordinate().ToPosition()) < radiusKM && 
                                                    !IsInExcluded(elem)).ToList();
    }
    
    private bool IsInExcluded(Camp camp)
    {
        return ExcludedCountries.Contains(camp.GetCountry());
    }
    
   
    

    
}