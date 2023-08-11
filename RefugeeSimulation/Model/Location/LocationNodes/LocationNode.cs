using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LaserTagBox.Model.Location.Camps;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Refugee;
using Mars.Components.Environments;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Location.LocationNodes;

public class LocationNode : IVectorFeature, ILocation
{
    public VectorStructuredData VectorStructured { get; private set; }
    
    

    public double Score { get; set; }
    
    public  int NumCamps { get; set; }

    public int NumConflicts { get; set; }

    public double NormNumCamps { get; set; }

    public double NormNumConflicts { get; set; }

    public double NormRefPop { get; set; }

    public double NormAnchorScore { get; set; }
    
    public double AnchorScore { get; private set; }

    public HashSet<LocationNode> Neighbours = new HashSet<LocationNode>();
    
    public int RefPop { get; set; }
    
    
    public Position Position { get; set; }

    public NodeLayer nodeLayer;
    
    public String Country { get; set; }
    
    







    

    

    


    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;

        

        var name = "Unknown";



        if (VectorStructured.Data.ContainsKey("ADM3_EN") && !(VectorStructured.Data["ADM3_EN"] is null))
            name = VectorStructured.Data["ADM3_EN"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM3_REF") && !(VectorStructured.Data["ADM3_REF"] is null))
            name = VectorStructured.Data["ADM3_REF"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM3ALT1EN") && !(VectorStructured.Data["ADM3ALT1EN"] is null))
            name = VectorStructured.Data["ADM3ALT1EN"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM3ALT2EN") && !(VectorStructured.Data["ADM3ALT2EN"] is null))
            name = VectorStructured.Data["ADM3ALT2EN"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM2_EN") && !(VectorStructured.Data["ADM2_EN"] is null))
            name = VectorStructured.Data["ADM2_EN"].ToString();
        else if (VectorStructured.Data.ContainsKey("adm2_en")&& !(VectorStructured.Data["adm2_en"] is null))
            name = VectorStructured.Data["adm2_en"].ToString();
        else if (VectorStructured.Data.ContainsKey("adm1_en") && !(VectorStructured.Data["adm1_en"] is null))
            name = VectorStructured.Data["adm1_en"].ToString();
        else if (VectorStructured.Data.ContainsKey("ADM1_EN") && !(VectorStructured.Data["ADM1_EN"] is null))
            name = VectorStructured.Data["ADM1_EN"].ToString();
        
        VectorStructured.Data.Add("Name", name);
        
        
        
        

        var country = "Unknown";

        if (VectorStructured.Data.ContainsKey("layer"))
        {
            if (!(VectorStructured.Data["layer"] is null) &&
                VectorStructured.Data["layer"].ToString().Contains("turkey"))
            {
                country = "Turkey";
            } else if (!(VectorStructured.Data["layer"] is null) &&
                       VectorStructured.Data["layer"].ToString().Contains("syria"))
            {
                country = "Syria";
            }
        } else if (VectorStructured.Data.ContainsKey("ADM0_EN") && !(VectorStructured.Data["ADM0_EN"] is null))
        {
            if (VectorStructured.Data["ADM0_EN"].ToString().EqualsIgnoreCase("Syrian Arab Republic"))
            {
                country = "Syria";
            } else if (VectorStructured.Data["ADM0_EN"].ToString().EqualsIgnoreCase("Turkey"))
            {
                country = "Turkey";
            }
        } else if(VectorStructured.Data.ContainsKey("adm0_en") && !(VectorStructured.Data["adm0_en"] is null && VectorStructured.Data["adm0_en"].ToString().EqualsIgnoreCase("Turkey")))

        {
            country = "Turkey";
        }

        

        Country = country;



        nodeLayer = (NodeLayer) layer;

        if (!(nodeLayer.CampLayer is null) && !(nodeLayer.ConflictLayer is null))
        {
            InitCamps(nodeLayer.CampLayer);
            InitConflicts(nodeLayer.ConflictLayer);
        }


        Position = Position.CreateGeoPosition(GetCentroidPosition().Longitude, GetCentroidPosition().Latitude);
       AnchorScore = Math.Sqrt(Math.Pow(Position.X - NodeLayer.AnchorCoordinates.X, 2) + Math.Pow(Position.Y - NodeLayer.AnchorCoordinates.Y, 2)
       
       );


    }

    public void InitConflicts(ConflictLayer conflictLayer)
    {
        var conflicts = conflictLayer.GetConflicts();

    
         foreach (var conflict in conflicts)
         {
             if (conflict.Month == nodeLayer.StartMonth &&
                 conflict.GetConflictGeometry().IsWithinDistance(VectorStructured.Geometry, 0))
             {
                 NumConflicts++;
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
        return VectorStructured.Data["Name"].ToString();
    }
    public string GetCountry()
    {
        return Country;
    }
    public Position GetCentroidPosition()
    {
        
        Point centroidPoint = VectorStructured.Geometry.Centroid;
        return new Position(centroidPoint.X, centroidPoint.Y);
    }

    public Geometry GetGeometry()
    {
       return VectorStructured.Geometry;
    }

    public Position GetPosition()
    {
        return Position;
    }

    public void Update(VectorStructuredData data)
    {
    }
    
    public int GetNumCampsAtNode()
    {
        return NumCamps;
    }

    public int GetNumConflictsAtNode()
    {
        return NumConflicts;
    }

    public HashSet<LocationNode> GetNeighbours()
    {
        return Neighbours;
    }

    public double GetScore()
    {
        return Score;
    }
    

    public void GetRandomRefugeesAtNode(GeoHashEnvironment<RefugeeAgent> environment)
    {
        ISocialNetwork[] refsAtNode = environment.Explore(Position, -1D, -1, elem => elem is not null &&
            elem.Position.DistanceInKmTo(Position) < 1).ToArray();
            
           
        if (refsAtNode.Length > 1)
        {
            var ref1 = refsAtNode[new Random().Next(refsAtNode.Length - 1)];
            var ref2 = ref1;

            while (ref2 == ref1)
            {
                ref2 = refsAtNode[new Random().Next(refsAtNode.Length )];
            }
            
            ref1.UpdateSocialNetwork(ref2);

        }
    }

    public void UpdateNormRefPop(int maxRefPop)
    {
        NormRefPop = RefPop * 1.0 / (maxRefPop * 1.0 );
    }
    
    
}