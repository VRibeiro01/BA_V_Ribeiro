using System;
using Mars.Common.IO.Mapped.Collections;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using RefugeeSimulation.Model.Model.Refugee;
using ServiceStack;

namespace LaserTagBox.Model.Model.Location.LocationNodes;

public class LocationNode : IVectorFeature
{
    public VectorStructuredData VectorStructured { get; private set; }
    
    private double Score { get; set; }

    private string Name { get; set; }

    private string Country { get; set; }

    private int NumCamps;

    private int NumConflicts;

    private double NormNumCamps { get; set; }

    private double NormNumConflicts { get; set; }

    private double NormRefPop { get; set; }

    private double NormAnchorScore { get; set; }

    private List<RefugeeAgent> RefugeesInLocation { get; set; }
    public void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;
        
        // TODO add normalized camp and conflict attributes and initialize them
        //TODO initialize location score (first check if necessary)

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
        
        VectorStructured.Data.Add("Residents", 0);

    }
    
    // TODO Safe erase methods
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
        throw new System.NotImplementedException();
    }
    
    public void EnterNewLocation() {}

    public void LeavePreviousLocation() {}

    public int GetNumCampsAtNode()
    {
        return 0;
    }

    public int GetNumConflictsAtNode()
    {
        return 0;
    }

    public List<LocationNode> GetNeighboursAndScores()
    {
        return null;
    }

    private void GetRandomRefugeesAtNode()
    {
        
    }

    private int UpdateNormRefPop(int maxRefPop)
    {
        return 0;
    }

    
}