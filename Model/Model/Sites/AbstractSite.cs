using System;
using System.Linq;
using Mars.Common.IO.Mapped.Collections;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;


namespace RefugeeSimulation.Model.Model.Sites;

public abstract class AbstractSite: IVectorFeature
{
    // data on sites from integrated data source
    public VectorStructuredData VectorStructured { get; private set; }
    
    public virtual void Init(ILayer layer, VectorStructuredData data)
    {
        VectorStructured = data;

        // Extract and set the site name
        var name = "Unknown";
        if (VectorStructured.Data.ContainsKey("name:en"))
            name = VectorStructured.Data["name:en"].ToString();
        else if (VectorStructured.Data.ContainsKey("name:de"))
            name = VectorStructured.Data["name:de"].ToString();
        else if (VectorStructured.Data.ContainsKey("name"))
            name = VectorStructured.Data["name"].ToString();
        else if (VectorStructured.Data.ContainsKey("name:ku"))
            name = VectorStructured.Data["name:ku"].ToString();
        
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
        VectorStructured.Data.Add("X", GetCoordinate().X);
        VectorStructured.Data.Add("Y", GetCoordinate().Y);
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
    
   
}

    
