﻿using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using ServiceStack;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public class LocationNode : IVectorFeature
{
    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;

        var name = "Unknown";
        var dupe = 0;


        if (VectorStructured.Data.ContainsKey("ADM3_EN"))
            name = VectorStructured.Data["ADM3_EN"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM3_REF"))
            name = VectorStructured.Data["ADM3_REF"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM3ALT1EN"))
            name = VectorStructured.Data["ADM3ALT1EN"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM3ALT2EN"))
            name = VectorStructured.Data["ADM3ALT2EN"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM2_EN"))
            name = VectorStructured.Data["ADM2_EN"].ToString();
        
        VectorStructured.Data.Add("Name", name);

      
        
    }
    public string GetName()
    {
        return VectorStructured.Data["Name"].ToString();
    }
    public Mars.Interfaces.Environments.Position GetCentroidPosition()
    {
        Point centroidPoint = VectorStructured.Geometry.Centroid;
        return new Mars.Interfaces.Environments.Position(centroidPoint.X, centroidPoint.Y);
    }



    public void Update(VectorStructuredData data)
    {
        throw new System.NotImplementedException();
    }

    public VectorStructuredData VectorStructured { get; private set; }
}