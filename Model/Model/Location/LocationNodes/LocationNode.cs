using System;
using System.Collections.Generic;
using System.Linq;
using LaserTagBox.Model.Model.Location.Camps;
using LaserTagBox.Model.Model.Location.Conflict;
using Mars.Components.Environments;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using RefugeeSimulation.Model.Model.Refugee;
using RefugeeSimulation.Model.Model.Shared;
using ServiceStack;
using Position = Mars.Interfaces.Environments.Position;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public class LocationNode : AbstractEnvironmentObject, IVectorFeature, ILocation
{
    public VectorStructuredData VectorStructured { get; private set; }

    public double Score { private get; set; }
    
    public  int NumCamps { get; set; }

    public int NumConflicts { get; set; }

    public double NormNumCamps { get; set; }

    public double NormNumConflicts { get; set; }

    public double NormRefPop { get; set; }

    public double NormAnchorScore { get; set; }
    
    public double AnchorScore { get; private set; }

    public List<ILocation> Neighbours = new List<ILocation>();
    
    public int RefPop { get; set; }

    public GeoHashEnvironment<AbstractEnvironmentObject> Environment;






    // Layers

    public ConflictLayer ConflictLayer => ConflictLayer.CreateInstance();

    public CampLayer CampLayer => CampLayer.CreateInstance();


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
        }
        
        VectorStructured.Data.Add("Country", country);
        
        
       InitCamps();
       InitConflicts();
       
       
       this.Position = Position.CreateGeoPosition(GetCentroidPosition().Longitude, GetCentroidPosition().Latitude);
       NodeLayer nodeLayer = (NodeLayer) layer;
       Environment = nodeLayer.GetEnvironment();
       

      
       AnchorScore = Math.Sqrt(Math.Pow(Position.X - NodeLayer.AnchorCoordinates.X, 2) + Math.Pow(Position.Y - NodeLayer.AnchorCoordinates.Y, 2)
       );


    }

    private void InitConflicts()
    {
        var conflicts = ConflictLayer.GetConflictCoordinates();

        foreach (var conflict in conflicts)
        {
            if (conflict.IsWithinDistance(VectorStructured.Geometry, 5))
            {
                NumConflicts++;
            }
        }
    }

    private void InitCamps()
    {
        var camps = CampLayer.GetCamps();

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
        return VectorStructured.Data["Country"].ToString();
    }
    public Mars.Interfaces.Environments.Position GetCentroidPosition()
    {
        
        Point centroidPoint = VectorStructured.Geometry.Centroid;
        return new Mars.Interfaces.Environments.Position(centroidPoint.X, centroidPoint.Y);
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

    public System.Collections.Generic.List<ILocation> GetNeighbours()
    {
        return Neighbours;
    }

    public double GetScore()
    {
        return Score;
    }
    

    public void GetRandomRefugeesAtNode()
    {
        ISocialNetwork[] refsAtNode = Environment.Explore(Position, 0.01, -1, elem => elem is ISocialNetwork)
           .Select(elem => (ISocialNetwork) elem).ToArray();
        if (refsAtNode.Length > 1)
        {
            var ref1 = refsAtNode[new Random().Next(refsAtNode.Length - 1)];
            var ref2 = ref1;

            while (ref2 == ref1)
            {
                ref2 = refsAtNode[new Random().Next(refsAtNode.Length - 1)];
            }
            
            ref1.updateSocialNetwork(ref2);

        }
    }

    public void UpdateNormRefPop(int maxRefPop)
    {
        NormRefPop = RefPop * 1.0 / maxRefPop;
    }
    
    
}