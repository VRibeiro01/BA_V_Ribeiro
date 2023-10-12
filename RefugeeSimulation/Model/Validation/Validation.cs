using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.LocationNodes;
using LaserTagBox.Model.Refugee;
using ServiceStack;

namespace LaserTagBox;

public static class Validation
{
    //
    public static Dictionary<Tuple<string, string>, int> Routes = new();


    public static Dictionary<string, int> TurkishDistrictsPop =
        new();
    
    public static Dictionary<string, int> SyrianDistrictsPop =
        new();
    
    public static Dictionary<string, int> TurkishDistrictsInitPop =
        new();
    
    public static Dictionary<string, int> SyrianDistrictsInitPop =
        new();

    public static int NumSteps = 0;

    public static int NumSimRuns = 0;

    public static int RefsSpawned;

    public static int RefsActivated;


    public static int HasConflictAndContacts;
    public static int HasConflictAndCamp;
    public static int OnlyHasCamp;
    public static int OnlyHasContacts;
    public static int OnlyHasConflict;
    public static int HasCampAndContacts;
    public static int HasNone;
    public static int HasAll;
    public static int PopOver50k;
    public static int PopUnder1k;
    public static int Pop20_50k;
    public static int Pop1_20k;
    
    public static int NumDecisions;

    public static double PercentageRefsActivated;


    public static void Print()
    {
        Console.WriteLine(
            "--------------------------------Validation Results-----------------\n" +
            "NumDecisions: " + NumDecisions + '\n' +
            "PercentageRefsActivated: " + CalcPercentageRefsActivated() + '\n' +
            "HasConflictAndContactsPercentage: " + HasConflictAndContacts * 1.0 / NumDecisions * 100 + '\n' +
            "HasConflictAndCampPercentage: " + HasConflictAndCamp * 1.0 / NumDecisions * 100 + '\n' +
            "OnlyHasCampPercentage: " + OnlyHasCamp * 1.0 / NumDecisions * 100 + '\n' +
            "OnlyHasContactsPercentage: " + OnlyHasContacts * 1.0 / NumDecisions * 100 + '\n' +
            "OnlyHasConflictPercentage: " + OnlyHasConflict * 1.0 / NumDecisions * 100 + '\n' +
            "HasCampAndContactsPercentage: " + HasCampAndContacts * 1.0 / NumDecisions * 100 + '\n' +
            "HasNonePercentage: " + HasNone * 1.0 / NumDecisions * 100 + '\n' +
            "HasAllPercentage: " + HasAll * 1.0 / NumDecisions * 100 + '\n' +
            "IDP Pop Under 1K Percentage: " + PopUnder1k * 1.0 / NumDecisions * 100 + '\n' +
            "IDP Pop 1-20K Percentage: " + Pop1_20k * 1.0 / NumDecisions * 100 + '\n' +
            "IDP Pop 20-50K Percentage: " + Pop20_50k * 1.0 / NumDecisions * 100 + '\n' +
            "IDP Pop Over 50K Percentage: " + PopOver50k * 1.0 / NumDecisions * 100 + '\n'
        );

       
    }

    public static double CalcPercentageRefsActivated()
    {
        PercentageRefsActivated = PercentageRefsActivated / (NumSteps * 1.0) * 100;
        return PercentageRefsActivated;
    }

    public static void IncrementPercentageActivatedRefs()
    {
        PercentageRefsActivated += RefsActivated * 1.0 / (RefsSpawned * 1.0);
        RefsActivated = 0;
    }

    public static void FillRoutes(List<MigrantAgent> agentsResult)
    {
        foreach (var agent in agentsResult)
        {
            if (!agent.OriginNode.GetName().EqualsIgnoreCase(agent.LocationName))
            {
                Tuple<string, string> route = new Tuple<String, String>(
                    agent.OriginNode.GetProvinceName(), agent.CurrentNode.GetProvinceName());


                if (Routes.ContainsKey(route))
                {
                    Routes[route]++;
                }
                else
                {
                    Routes.Add(route, 1);
                }
            }
        }
    }

    public static void FillTurkishDistrictsPop(List<LocationNode> districts)
    {
        var turkeyDistricts = districts;
        foreach (var district in turkeyDistricts)
        {
            var name = district.GetName(); //GetName()
            if (!TurkishDistrictsPop.ContainsKey(name))
            {
                TurkishDistrictsPop.Add(name, district.RefPop);
            }
            else
            {
                TurkishDistrictsPop[name] += district.RefPop;
            }
            
        }
    }
    public static void FillSyrianDistrictsPop(List<LocationNode> districts)
    {
        var syrianDistricts = districts;
        foreach (var district in syrianDistricts)
        {
            var name = district.GetProvinceName(); //GetName()
            if (!SyrianDistrictsPop.ContainsKey(name))
            {
                SyrianDistrictsPop.Add(name, district.RefPop);
            }
            else
            {
                SyrianDistrictsPop[name] += district.RefPop;
            }
            
        }
    }
    
    public static void FillTurkishDistrictsInitPop(List<LocationNode> districts)
    {
        if (TurkishDistrictsInitPop.Count > 0) return;
        var turkeyDistricts = districts;
        foreach (var district in turkeyDistricts)
        {
            var name = district.GetName();
            if(!TurkishDistrictsInitPop.ContainsKey(name))
                TurkishDistrictsInitPop.Add(name, district.RefPop);
            else
            {
                TurkishDistrictsInitPop[name]+= district.RefPop;
            }
        }
    }
    
    public static void FillSyrianDistrictsInitPop(List<LocationNode> districts)
    {
        if (SyrianDistrictsInitPop.Count > 0) return;
        var syrianDistricts = districts;
        foreach (var district in syrianDistricts)
        {
            var name = district.GetProvinceName();
            if(!SyrianDistrictsInitPop.ContainsKey(name))
            SyrianDistrictsInitPop.Add(name, district.RefPop);
            else
            {
                SyrianDistrictsInitPop[name]+= district.RefPop;
            }
        }
    }

    public static void CalcAverageDistribution()
    {
        foreach (var key in TurkishDistrictsPop.Keys.ToList())
        {
            TurkishDistrictsPop[key]   /= NumSimRuns;
        }
        
        foreach (var key in Routes.Keys.ToList())
        {
            Routes[key] /= NumSimRuns;
        }
    }
    

    public static void WriteToFileTurkey(string identifier)
    {
        var docPath = "Model/Validation";
            File.WriteAllText(Path.Combine(docPath,"InitPop.csv"),"Region,InitPop\n");
        foreach (var districtPopPair in TurkishDistrictsInitPop)
        {
            File.AppendAllText(Path.Combine(docPath,"InitPop.csv"),districtPopPair.Key+","+districtPopPair.Value+'\n');
        }
        
        
        File.WriteAllText(Path.Combine(docPath,"RefPop"+identifier+".csv"),"Region,RefPop\n");
        foreach (var districtPopPair in TurkishDistrictsPop)
        {
            File.AppendAllText(Path.Combine(docPath,"RefPop"+identifier+".csv"),districtPopPair.Key+","+districtPopPair.Value+'\n');
        }
        
        File.WriteAllText(Path.Combine(docPath,"Routes"+identifier+".csv"),"Origin,Destination,Number\n");
        foreach (var routeNumberPair in Routes)
        {
            File.AppendAllText(Path.Combine(docPath,"Routes"+identifier+".csv"),
                routeNumberPair.Key.Item1+","+
                routeNumberPair.Key.Item2+"," + 
                routeNumberPair.Value+'\n');
        }
        
    }
    
    public static void WriteToFileSyria(string identifier)
    {
        var docPath = "Model/Validation";
        File.WriteAllText(Path.Combine(docPath,"SyrInitPop.csv"),"Region,InitPop\n");
        foreach (var districtPopPair in SyrianDistrictsInitPop)
        {
            File.AppendAllText(Path.Combine(docPath,"SyrInitPop.csv"),districtPopPair.Key+","+districtPopPair.Value+'\n');
        }
        
        
        File.WriteAllText(Path.Combine(docPath,"SyrRefPop"+identifier+".csv"),"Region,RefPop\n");
        foreach (var districtPopPair in SyrianDistrictsPop)
        {
            File.AppendAllText(Path.Combine(docPath,"SyrRefPop"+identifier+".csv"),districtPopPair.Key+","+districtPopPair.Value+'\n');
        }
        
        File.WriteAllText(Path.Combine(docPath,"SyrRoutes"+identifier+".csv"),"Origin,Destination,Number\n");
        foreach (var routeNumberPair in Routes)
        {
            File.AppendAllText(Path.Combine(docPath,"SyrRoutes"+identifier+".csv"),
                routeNumberPair.Key.Item1+","+
                routeNumberPair.Key.Item2+"," + 
                routeNumberPair.Value+'\n');
        }
        
    }
}