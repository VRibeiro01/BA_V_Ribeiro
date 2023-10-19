using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Location;

public class Camp : IVectorFeature
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
        return VectorStructured.Geometry;
    }

    public VectorStructuredData VectorStructured { get; private set; }
}