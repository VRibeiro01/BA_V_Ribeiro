

using System.Collections.Generic;

namespace LaserTagBox.Model.Location.LocationNodes;

public interface ILocation
{
    /// <summary>
    ///  Returns the number of active camps present at the location node 
    /// </summary>
    /// <returns>int</returns>
    public int  GetNumCampsAtNode();
    
    /// <summary>
    ///  Returns the number of conflicts taking place within 5 km of the node in the time period of the simulation
    /// </summary>
    /// <returns>int</returns>
    public int GetNumConflictsAtNode();


    /// <summary>
    ///  Returns a list of the location nodes within a radius of 100 km.
    /// If there are no neighbours, then an empty list is returned.
    /// </summary>
    /// <returns> list of neighbouring location nodes </returns>
    public List<ILocation> GetNeighbours();
    
    
    /// <summary>
    /// Return the previously calculated location score of the node 
    /// </summary>
    /// <returns> double </returns>
    public double GetScore();
    
    /// <summary>
    ///  Return the name of this location
    /// </summary>
    /// <returns> string</returns>

    public string GetName();


    public Mars.Interfaces.Environments.Position GetCentroidPosition();



}