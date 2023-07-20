using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Model.Location.Conflict;

public class Conflict : IVectorFeature
{
    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;
    }

    public void Update(VectorStructuredData data)
    {
        
    }

    public Geometry GetCoordinates()
    {
        return this.GetCoordinates();
    }
    public VectorStructuredData VectorStructured { get; private set; }
}