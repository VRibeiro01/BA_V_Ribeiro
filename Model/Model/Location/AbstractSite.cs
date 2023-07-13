using System;
using System.Linq;
using Mars.Common.IO.Mapped.Collections;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;

namespace LaserTagBox.Model.Model.Location;

public abstract class AbstractSite: IVectorFeature
{
    // data on sites from integrated data source
    public VectorStructuredData VectorStructured { get; private set; }
    private int refugeePopulation;
    
    public virtual void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;

        
        
        var name = "Unknown";
       
       
       
       
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
        
        var country = "";
        if (VectorStructured.Data.ContainsKey("country"))
        {
            country = (string) VectorStructured.Data["country"];
        }
        VectorStructured.Data.Add("Country", country);
        
        // Extract and set the population/residents of the site
        var population = 0;
        if (VectorStructured.Data.ContainsKey("population"))
        {
            population = int.Parse((string) VectorStructured.Data["population"]);
        }
        VectorStructured.Data.Add("Residents", population);
        
            
        // Creates a dictionary with only the most important information on sites
        var allowedKeys = new List<string> {"Name", "Country", "Residents" };
        VectorStructured.Data = VectorStructured.Data
            .Where(elem => allowedKeys.Contains(elem.Key))
            .ToDictionary(elm => elm.Key, elm => elm.Value);
            
        // Add Lat, Lon & ID as to normalize the Data
        VectorStructured.Data.Add("X", GetCoordinate()[0]);
        VectorStructured.Data.Add("Y", GetCoordinate()[1]);
        VectorStructured.Data.Add("ID", Guid.NewGuid());
    }

    public void Update(VectorStructuredData data)
    {
        throw new NotImplementedException();
    }
    
    public string GetName()
    {
        return VectorStructured.Data["Name"].ToString();
    }
    
    public abstract String GetCountry();

    
    // Get the coordinate of the site
    public abstract Coordinate GetCoordinate();

    public int GetResidents()
    {
       return  (int)VectorStructured.Data["Residents"];
    }

    public void EnterCity()
    {
        VectorStructured.Data["Residents"] = (int)VectorStructured.Data["Residents"]+1;
    }
    
   
}

    
