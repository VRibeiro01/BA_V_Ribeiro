using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Model.Location.Conflict;

public class Conflict : IVectorFeature
{
    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;
    }

    public void Update(VectorStructuredData data)
    {
        throw new System.NotImplementedException();
    }

    public VectorStructuredData VectorStructured { get; private set; }
}