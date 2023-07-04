using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace RefugeeSimulation.Model.Model.Sites;

public class Camp : AbstractSite
{
   

    public override string GetCountry()
    {
        return VectorStructured.Data["Country"] switch
        {
            "sy" => "Syria",
            "jo" => "Jordan",
            "tr" => "Turkey",
            "iq" => "Iraq",
            "lb" => "Lebanon",
            _ => "Unknown"
        };
    }

    public override Coordinate GetCoordinate()
    {
        return VectorStructured.Geometry.Coordinate;
    }
}