using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Model.Location.Camps;

public class Camp : IVectorFeature
{
    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;
        var name = "Unknown";
        
        if (VectorStructured.Data.ContainsKey("adm2_en"))
            name = VectorStructured.Data["adm2_en"].ToString();
     
        
        VectorStructured.Data.Add("Name", name);

    }

    public void Update(VectorStructuredData data)
    {
        throw new System.NotImplementedException();
    }

    public Geometry GetCoordinates()
    {
        return this.GetCoordinates();
    }

    public VectorStructuredData VectorStructured { get; private set; }
}