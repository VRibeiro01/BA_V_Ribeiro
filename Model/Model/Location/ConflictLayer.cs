using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Mars.Common.Data;
using Mars.Components.Layers;
using Mars.Interfaces;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace LaserTagBox.Model.Model.Location;

public class ConflictLayer : VectorLayer
{
    private ISimulationContext _simulationContext;
    private ConflictsData _conflictsData;

  
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgentHandle = null)
    {
        var initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

        _simulationContext = layerInitData.Context;

        var jsonString = layerInitData.LayerInitConfig.Inputs[0].Import().First().ToString();
        _conflictsData = JsonSerializer.Deserialize<ConflictsData>(jsonString);

        foreach (var city in _conflictsData.Cities)
        {
            foreach (var conflict in city.Conflicts)
            {
                conflict.End = new DateTime(
                    conflict.End.Year,
                    conflict.End.Month,
                    DateTime.DaysInMonth(conflict.End.Year, conflict.End.Month),
                    23,
                    59,
                    59
                );
                if (conflict.End < conflict.Start)
                    throw new InvalidDataException("Conflict End can not be before the Start for City: " + city.Name +
                                                   " Data: " + conflict);
            }
        }

        return initiated;
    }

    public bool GetConflictStateForCity(string name)
    {
        var simulationDateTime = _simulationContext.CurrentTimePoint ?? new DateTime();
        var city = _conflictsData.Cities.FirstOrDefault(elm => elm.Name.Equals(name));
        var conflictEntry =
            city?.Conflicts.FirstOrDefault(elm => elm.Start <= simulationDateTime && elm.End >= simulationDateTime);
        return conflictEntry != null;
    }

    public class ConflictsData
    {
        public string Version { get; set; }
        public IList<ConflictsCityData> Cities { get; set; }
    }

    public class ConflictsCityData
    {
        public string Name { get; set; }

        public IList<ConflictsCityConflictEntry> Conflicts { get; set; }
    }

    public class ConflictsCityConflictEntry
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}