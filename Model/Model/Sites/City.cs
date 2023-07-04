using System;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;


namespace RefugeeSimulation.Model.Model.Sites;

public class City : AbstractSite
{
    private CityLayer CityLayer { get; set; }

    public override void Init(ILayer layer, VectorStructuredData data)
    {
        base.Init(layer, data);
        CityLayer = (CityLayer) layer;
        VectorStructured.Data["Country"] = "Syria";
        if ((int)VectorStructured.Data["Residents"] == 0)
        {
            VectorStructured.Data["Residents"] = 1000;
        }
    }

    public override string GetCountry()
    {
        return (String)VectorStructured.Data["Country"];
    }

    public override Coordinate GetCoordinate()
    {
        return VectorStructured.Geometry.Coordinate;
    }

    
}