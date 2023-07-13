using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Model.Location;

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
    
    public Geometry GetCoordinates()
    {
        var cor = VectorStructured.Geometry.Coordinates;
        return VectorStructured.Geometry;
    }
    
    public Coordinate GetCentroid()
    {
        Point centroidPoint = VectorStructured.Geometry.Centroid;
        return new Coordinate(centroidPoint.X, centroidPoint.Y);
    }
}