using System;
using System.Collections.Generic;
using System.Linq;
using LaserTagBox.Model.Location.Camps;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Refugee;
using Mars.Components.Environments;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Location.LocationNodes;

public class Location : IVectorFeature
{
    

    //------------------------------- Parameters needed to calculate location scores ------------------------
    public double Score { get; set; }

    public int NumCamps { get; set; }

    public int NumConflicts { get; set; }
    
    public int RefPop;
    
    
    public HashSet<Location> Neighbours = new();

    public double NormNumCamps { get; set; }

    public double NormNumConflicts { get; set; }

    public double NormRefPop { get; set; }

    public double NormAnchorScore { get; set; }

    public double AnchorScore { get; private set; }
    
    


    // -----------------------------------------Layers -----------------------------------------------------------
    public LocationLayer LocationLayer;
    
    //------------------------ Variables for Syria IDP experiment: Permanent population number of location ----------------------------------------------
    
    public int Population { get; set; }
    public double NormPop { get; set; }
    
    // ----------------------------------------------------------------------------------------------
    public VectorStructuredData VectorStructured { get; private set; }
    public Position Position { get; set; }
    public String Country { get; set; }

    


    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;

// Extract names of administrative levels
        var name1 = "Unknown";
        var name2 = "Unknown";
        var name3 = "Unknown";


        if (VectorStructured.Data.ContainsKey("ADM3_EN") && !(VectorStructured.Data["ADM3_EN"] is null))
        {
            name3 = VectorStructured.Data["ADM3_EN"].ToString();
        }
        else if (VectorStructured.Data.ContainsKey("ADM3_REF") && !(VectorStructured.Data["ADM3_REF"] is null))
        {
            name3 = VectorStructured.Data["ADM3_REF"].ToString();
        }
        else if (VectorStructured.Data.ContainsKey("ADM3ALT1EN") && !(VectorStructured.Data["ADM3ALT1EN"] is null))
        {
            name3 = VectorStructured.Data["ADM3ALT1EN"].ToString();
        }
        else if (VectorStructured.Data.ContainsKey("ADM3ALT2EN") && !(VectorStructured.Data["ADM3ALT2EN"] is null))
        {
            name3 = VectorStructured.Data["ADM3ALT2EN"].ToString();
        }

        if (VectorStructured.Data.ContainsKey("ADM2_EN") && !(VectorStructured.Data["ADM2_EN"] is null))
        {
            name2 = VectorStructured.Data["ADM2_EN"].ToString();
        }
        else if (VectorStructured.Data.ContainsKey("adm2_en") && !(VectorStructured.Data["adm2_en"] is null))
        {
            name2 = VectorStructured.Data["adm2_en"].ToString();
        }

        if (VectorStructured.Data.ContainsKey("adm1_en") && !(VectorStructured.Data["adm1_en"] is null))
        {
            name1 = VectorStructured.Data["adm1_en"].ToString();
        }
        else if (VectorStructured.Data.ContainsKey("ADM1_EN") && !(VectorStructured.Data["ADM1_EN"] is null))
        {
            name1 = VectorStructured.Data["ADM1_EN"].ToString();
        }

        VectorStructured.Data.Add("Name1", name1);
        VectorStructured.Data.Add("Name2", name2);
        VectorStructured.Data.Add("Name3", name3);


        LocationLayer = (LocationLayer) layer;

        if (!(LocationLayer.CampLayer is null) && !(LocationLayer.ConflictLayer is null))
        {
            InitCamps(LocationLayer.CampLayer);
            InitConflicts(LocationLayer.ConflictLayer);
        }


        Position = Position.CreatePosition(GetCentroidPosition().Longitude, GetCentroidPosition().Latitude);
        AnchorScore = Math.Sqrt(Math.Pow(GetCentroidPosition().Longitude - LocationLayer.AnchorCoordinates.X, 2) +
                                Math.Pow(GetCentroidPosition().Latitude - LocationLayer.AnchorCoordinates.Y, 2)
        );
       
    }

    public void InitConflicts(ConflictLayer conflictLayer)
    {
        var conflicts = conflictLayer.GetConflicts();
        NumConflicts = 0;


        foreach (var conflict in conflicts)
        {
            if (LocationLayer.Mode.EqualsIgnoreCase("Syria"))
            {
                if (conflict.Day == LocationLayer.GetCurrentTimePoint().Day
                    && 
                    conflict.GetCoordinates().IsWithinDistance(VectorStructured.Geometry, 0))
                {
                    NumConflicts++;
                } 
            }
            else
            {
                if (conflict.Month >= LocationLayer.StartMonth && conflict.Month <= LocationLayer.EndMonth&&
                    conflict.GetCoordinates().IsWithinDistance(VectorStructured.Geometry, 0))
                {
                    NumConflicts++;
                }
            }
        }
        
    }

    public void InitCamps(CampLayer campLayer)
    {
        var camps = campLayer.GetCamps();

        foreach (var camp in camps)
        {
            if (camp.IsWithinDistance(VectorStructured.Geometry, 0))
            {
                NumCamps++;
            }
        }
    }


    public string GetName()
    {
        if (!VectorStructured.Data["Name3"].ToString().EqualsIgnoreCase("Unknown"))
        {
            return VectorStructured.Data["Name3"].ToString();
        }

        if (!VectorStructured.Data["Name2"].ToString().EqualsIgnoreCase("Unknown"))
        {
            return VectorStructured.Data["Name2"].ToString();
        }

        return VectorStructured.Data["Name1"].ToString();
    }
    
    public string GetName2()
    {
        
        if (!VectorStructured.Data["Name2"].ToString().EqualsIgnoreCase("Unknown"))
        {
            return VectorStructured.Data["Name2"].ToString();
        }

        throw new Exception("No name for adm2");
    }

    public string GetProvinceName()
    {
        if (!VectorStructured.Data["Name1"].ToString().EqualsIgnoreCase("Unknown"))
        {
            return VectorStructured.Data["Name1"].ToString();
        }

        if (!VectorStructured.Data["Name2"].ToString().EqualsIgnoreCase("Unknown"))
        {
            return VectorStructured.Data["Name2"].ToString();
        }

        return VectorStructured.Data["Name3"].ToString();
    }

    public Position GetCentroidPosition()
    {
        Point centroidPoint = VectorStructured.Geometry.Centroid;
        return new Position(centroidPoint.X, centroidPoint.Y);
    }

   
    public void Update(VectorStructuredData data){ }


    public void GetRandomRefugeesAtNode(GeoHashEnvironment<MigrantAgent> environment)
    {
        MigrantAgent[] refsAtNode = environment.Explore(Position, -1D, -1, elem => elem is not null &&
            elem.Position.DistanceInKmTo(Position) < 1).ToArray();


        if (refsAtNode.Length > 1)
        {
            var ref1 = refsAtNode[new Random().Next(refsAtNode.Length - 1)];
            var ref2 = ref1;

            while (ref2 == ref1)
            {
                ref2 = refsAtNode[new Random().Next(refsAtNode.Length)];
            }

            ref1.UpdateSocialNetwork(ref2);
        }
    }

    public void UpdateNormRefPop(int maxRefPop)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        
            NormRefPop = RefPop * 1.0 / (maxRefPop * 1.0);
        
    }
}