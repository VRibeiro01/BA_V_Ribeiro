using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LaserTagBox.Model.Refugee;
using ServiceStack;

namespace LaserTagBox;

public class Validation
{
    //
    public static Dictionary<Tuple<string,string>, int> Routes = new Dictionary<Tuple<string, string>, int>();

   
    public static Hashtable TurkishDistrictsPop;
    
    public static int NumRuns;

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
            "HasConflictAndContactsPercentage: " + HasConflictAndContacts*1.0/NumDecisions*100 + '\n' +
            "HasConflictAndCampPercentage: " + HasConflictAndCamp*1.0/NumDecisions*100 + '\n' +
            "OnlyHasCampPercentage: " + OnlyHasCamp*1.0/NumDecisions*100 + '\n' + 
            "OnlyHasContactsPercentage: " + OnlyHasContacts*1.0/NumDecisions*100 + '\n' + 
            "OnlyHasConflictPercentage: " + OnlyHasConflict*1.0/NumDecisions*100 + '\n' + 
            "HasCampAndContactsPercentage: " + HasCampAndContacts*1.0/NumDecisions*100 + '\n' +
            "HasNonePercentage: " + HasNone*1.0/NumDecisions*100 + '\n' +
            "HasAllPercentage: " + HasAll*1.0/NumDecisions*100 + '\n' 
        );
        
        Console.WriteLine(
            "----------------Routes -----------------"
        );
        //Routes.Select(i => $"{i.Key} => {i.Value}").ToList().ForEach(Console.WriteLine);
        Routes.Select(
            i
                => string.Join(",",i.Key)+" => "+ i.Value).ToList().ForEach(Console.WriteLine);
    }

    public static double CalcPercentageRefsActivated()
    {
        PercentageRefsActivated = PercentageRefsActivated  / (NumRuns * 1.0) * 100;
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
            if(!agent.OriginNode.GetName().EqualsIgnoreCase(agent.LocationName)){
                
            Tuple<string, string> route = new Tuple<String, String>(
              agent.OriginNode.GetName(), agent.LocationName);

            
            if (Routes.ContainsKey(route))
            {
                Routes[route]++;
            }
            else
            {
                Routes.Add(route,1);
            }
            }
            
        }
        
        
    }

    //TODO num refs that crossed into Turkey
}