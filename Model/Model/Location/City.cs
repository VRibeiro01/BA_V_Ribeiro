using System;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Model.Location;

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

    public Geometry GetCoordinates()
    {
        var cor = VectorStructured.Geometry.Coordinates;
        return VectorStructured.Geometry;
    }

    public override Coordinate GetCoordinate()
    {
        return VectorStructured.Geometry.Coordinate;
    }

    public Mars.Interfaces.Environments.Position GetCentroidPosition()
    {
        Point centroidPoint = VectorStructured.Geometry.Centroid;
        return new Mars.Interfaces.Environments.Position(centroidPoint.X, centroidPoint.Y);
    }

    
}