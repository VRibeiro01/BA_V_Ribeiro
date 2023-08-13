using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Location.Conflict;

public class Conflict : IVectorFeature
{
    public long Month { get; set; }

    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;

        Month = (long) VectorStructured.Data["month"];
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