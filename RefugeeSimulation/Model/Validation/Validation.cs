using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.LocationNodes;
using LaserTagBox.Model.Refugee;
using ServiceStack;

namespace LaserTagBox;

public class Validation
{
    //
    public static Dictionary<Tuple<string, string>, int> Routes = new();


    public static Dictionary<string, int> TurkishDistrictsPop =
        new();
    
    public static Dictionary<string, int> TurkishDistrictsInitPop =
        new();

    public static int NumRuns = 0;

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
            "HasAllPercentage: " + HasAll * 1.0 / NumDecisions * 100 + '\n'
        );

       
    }

    public static double CalcPercentageRefsActivated()
    {
        PercentageRefsActivated = PercentageRefsActivated / (NumRuns * 1.0) * 100;
        return PercentageRefsActivated;
    }

    public static void IncrementPercentageActivatedRefs()
    {
        PercentageRefsActivated += RefsActivated * 1.0 / (RefsSpawned * 1.0);
        RefsActivated = 0;
    }

    public static void FillRoutes(List<RefugeeAgent> agentsResult)
    {
        foreach (var agent in agentsResult)
        {
            if (!agent.OriginNode.GetName().EqualsIgnoreCase(agent.LocationName))
            {
                Tuple<string, string> route = new Tuple<String, String>(
                    agent.OriginNode.GetName(), agent.LocationName);


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
        var turkeyDistricts = districts.Where(d => d.Country.EqualsIgnoreCase("Turkey"));
        foreach (var district in turkeyDistricts)
        {
            var name = district.GetName();
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
    
    public static void FillTurkishDistrictsInitPop(List<LocationNode> districts)
    {
        if (TurkishDistrictsInitPop.Count > 0) return;
        var turkeyDistricts = districts.Where(d => d.Country.EqualsIgnoreCase("Turkey"));
        foreach (var district in turkeyDistricts)
        {
            var name = district.GetName();
            TurkishDistrictsInitPop.Add(name, district.RefPop);
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
    

    public static void WriteToFile(int numRuns)
    {
        var docPath = "Model/Validation";
            File.WriteAllText(Path.Combine(docPath,"InitPop.csv"),"Region,InitPop\n");
        foreach (var districtPopPair in TurkishDistrictsInitPop)
        {
            File.AppendAllText(Path.Combine(docPath,"InitPop.csv"),districtPopPair.Key+","+districtPopPair.Value+'\n');
        }
        
        
        File.WriteAllText(Path.Combine(docPath,"RefPop"+numRuns+".csv"),"Region,RefPop\n");
        foreach (var districtPopPair in TurkishDistrictsPop)
        {
            File.AppendAllText(Path.Combine(docPath,"RefPop"+numRuns+".csv"),districtPopPair.Key+","+districtPopPair.Value+'\n');
        }
        
        File.WriteAllText(Path.Combine(docPath,"Routes"+numRuns+".csv"),"Origin,Destination,Number\n");
        foreach (var routeNumberPair in Routes)
        {
            File.AppendAllText(Path.Combine(docPath,"Routes"+numRuns+".csv"),
                routeNumberPair.Key.Item1+","+
                routeNumberPair.Key.Item2+"," + 
                routeNumberPair.Value+'\n');
        }
        
    }
}