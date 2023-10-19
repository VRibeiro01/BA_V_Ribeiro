using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location;
using LaserTagBox.Model.Refugee;
using Mars.Components.Agents;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Microsoft.CodeAnalysis.CSharp;
using ServiceStack;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

/* Test that locations and agents(agents are accessible in Environment --> done
   Test that location params are initialized correctly --> done
   Test that CalcScore works properly--> done
   Test that MaxRefPop works properly --> done
hgh*/
public class SimulationTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private LocationLayer _locationLayer;
    private ConflictLayer _conflictLayer;
    private CampLayer _campLayer;
    private PopulationLayer _populationLayer;


    public SimulationTest(ITestOutputHelper testOutputHelper)
    {
        string basePath = Directory.GetParent(
                Directory.GetParent(
                    Directory.GetParent(
                        Directory.GetParent(
                            Directory.GetCurrentDirectory()
                        ).FullName).FullName
                ).FullName).FullName;

        string testsPath = Path.Combine(basePath, "Tests");
        string rPath = Path.Combine(basePath, "RefugeeSimulation\\Resources");
            
        _testOutputHelper = testOutputHelper;
        
        
        _locationLayer = new LocationLayer();
        _populationLayer = new PopulationLayer();
        _locationLayer.PopulationLayer = _populationLayer;
        _locationLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine(testsPath, "selected_districts_for_test.geojson")
            }
        });

       

        _conflictLayer = new ConflictLayer();
        _conflictLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine(
                   rPath ,"conflicts_syria_17.geojson")
            }
        });


        _campLayer = new CampLayer();
        _campLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine(
                    rPath,"turkey_camps_idps.geojson")
            }
        });
        
    }

    [Fact]
    public void EnvironmentTest()
    {
        var agent = new MigrantAgent();
        Location moveTest = _locationLayer.Entities.First();
        
        foreach (var node in _locationLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                agent.Spawn(node);
                agent.Environment = _locationLayer.GetEnvironment();
                _locationLayer.GetEnvironment().Insert(agent);
            }
            
            var agent1 = new MigrantAgent();
            agent1.Spawn(node);
            _locationLayer.GetEnvironment().Insert(agent1);
        }

        var environment = _locationLayer.GetEnvironment();
        MigrantAgent[] refsAtNode = environment.Explore(_locationLayer.GetLocationByName("Tell Abiad").Position,
            -1D, -1, elem => elem is not null &&
                             elem.Position.DistanceInKmTo(_locationLayer.GetLocationByName("Tell Abiad").Position) < 1).ToArray();

        // Act
       
       agent.MoveToNode(moveTest);
        MigrantAgent[] refsAtNodeAfterMoving = environment.Explore(_locationLayer.GetLocationByName("Tell Abiad").Position,
            -1D, -1, elem => elem is not null &&
            elem.Position.DistanceInKmTo(_locationLayer.GetLocationByName("Tell Abiad").Position) < 1).ToArray();
        
        
        var agentsInEnvironment = environment.Explore().ToList();


        // Assert
        Assert.True(agentsInEnvironment.Count == 5);
        Assert.True(refsAtNode.Length==2);
        Assert.True(refsAtNodeAfterMoving.Length==1);
    }

    [Fact]
    public void LocationInitializationTest()
    {
        // Arrange

        _locationLayer.StartMonth = 9;
        _locationLayer.EndMonth = 9;
        

        // Act

        foreach (var node in _locationLayer.Entities)
        {
            node.InitConflicts(_conflictLayer);
            node.InitCamps(_campLayer);
        }

        _locationLayer.InitLocationParams();
        _locationLayer.UpdateNodeScores();
        


        //Assert

        foreach (var node in _locationLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                _testOutputHelper.WriteLine("Camps: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbors: " + node.Neighbors.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 0);
                Assert.True(node.NumCamps == 0);
                Assert.True(Math.Abs(node.NormNumConflicts - (0.0 / 2.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (0.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.Neighbors.Count == 3);
                
            }


            if (node.GetName().EqualsIgnoreCase("Lower Shyookh"))
            {
                _testOutputHelper.WriteLine("Camps 2: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 2: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbors 2: " + node.Neighbors.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 0);
                Assert.True(node.NumCamps == 2);
                Assert.True(Math.Abs(node.NormNumConflicts - (0.0 / 2.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (2.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Neighbors.Count > 0);
            }

            if (node.GetName().EqualsIgnoreCase("Abu Qalqal"))
            {
                _testOutputHelper.WriteLine("Camps 3: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 3: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbors 3: " + node.Neighbors.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 0);
                Assert.True(node.NumCamps == 0);
                Assert.True(Math.Abs(node.NormNumConflicts - (0.0 / 2.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (0.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.Neighbors.Count == 3);
            }

            if (node.GetName().EqualsIgnoreCase("Jarablus"))
            {
                _testOutputHelper.WriteLine("Camps 4: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 4: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbors 4: " + node.Neighbors.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 1);
                Assert.True(node.NumCamps == 2);
                Assert.True(Math.Abs(node.NormNumConflicts - (1.0 / 2.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (2.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.Neighbors.Count == 3);
            }
        }
    }

    [Fact]
    public void PopulationParameterTest()
    {

        //-------------------Test Initialization of Population------------------
        //Arrange
        _locationLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources",
                    "syrian_districts_3.geojson")
            }
        });
        
    
       var  populationLayer = new PopulationLayer();
        populationLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine(
                    "C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\syrPop_adm3.csv")
            }
        });
        
        _locationLayer.Mode = "Syria";
        _locationLayer.PopulationLayer = populationLayer;
        //Act
        _locationLayer.InitLocationParams();
        var testNode = _locationLayer.GetLocationByName("Tell Abiad");
        
        //Assert
        Assert.True(testNode.Population > 0);
        Assert.Equal(44671,testNode.Population);
        Assert.True(testNode.NormPop > 0 && testNode.NormPop<1);
        
    }

    [Fact]
    public void UpdateNodeScoresTest()
    {
        //Arrange


        foreach (var node in _locationLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                var agent = new MigrantAgent();
                agent.Spawn(node);
                _locationLayer.GetEnvironment().Insert(agent);
            }

            var agent1 = new MigrantAgent();
            agent1.Spawn(node);
            _locationLayer.GetEnvironment().Insert(agent1);
        }


        Location? updateNormRefPopTestNode = _locationLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));

        var environment = _locationLayer.GetEnvironment();


        // Act

        var calculatedMaxRefPop = _locationLayer.MaxRefPop();
        var agents = environment.Explore(new Position(), -1D, -1, a => a != null);
        var environmentObjects = agents.ToList();
        updateNormRefPopTestNode.UpdateNormMigPop(calculatedMaxRefPop);

        //Assert
        _testOutputHelper.WriteLine(environmentObjects.Count.ToString());
        Assert.True(environmentObjects.ToList().Count == 5);
        Assert.Equal(2, calculatedMaxRefPop);
        Assert.Equal(1.0, updateNormRefPopTestNode.NormMigPop);
    }

    [Fact]
    public void UpdateSocialNetworkTest()
    {
        //Arrange


        var agent = new MigrantAgent();
        var agent1 = new MigrantAgent();
        foreach (var node in _locationLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                agent.Spawn(node);
                _locationLayer.GetEnvironment().Insert(agent);
                agent.Friends = new HashSet<MigrantAgent>();
            }
            
            agent1.Spawn(node);
            _locationLayer.GetEnvironment().Insert(agent1);
            agent1.Friends = new HashSet<MigrantAgent>();
        }


        var environment = _locationLayer.GetEnvironment();
        var testNode = _locationLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));


        //act
        testNode.GetRandomAgentsAtNode(environment);

        //Assert

        Assert.Contains(environment.Explore().Select(e => (MigrantAgent) e), e => !e.Friends.IsEmpty());
    }


    [Fact]
    public void RefugeeNumContactsAtNode()
    {
        //Arrange


        var agent = new MigrantAgent();
        var agent1 = new MigrantAgent();
        var environment = _locationLayer.GetEnvironment();
        var agentList = new List<MigrantAgent>();
        foreach (var node in _locationLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                agent.Spawn(node);
                _locationLayer.GetEnvironment().Insert(agent);
                agent.Friends = new HashSet<MigrantAgent>();
                agent.Environment = environment;
                agentList.Add(agent);
            }
            
            agent1.Spawn(node);
            _locationLayer.GetEnvironment().Insert(agent1);
            agent1.Friends = new HashSet<MigrantAgent>();
            agent1.Environment = environment;
            agentList.Add(agent1);
        }


        var testNode = _locationLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));
       


        //act


        testNode.GetRandomAgentsAtNode(environment);
        int calcNumFriendsAtNode = agent.GetNumFriendsAtNode(testNode);
        int calcNumFriendsAtNode1 = agent1.GetNumFriendsAtNode(testNode);


        //Assert


        Assert.Equal(1, calcNumFriendsAtNode);
        Assert.Equal(1, calcNumFriendsAtNode1);
    }


  [Fact]
    public void RefPopTest()
    {
        //Arrange


        var agent = new MigrantAgent();
        var agent1 = new MigrantAgent();
        var environment = _locationLayer.GetEnvironment();
        var agentList = new List<MigrantAgent>();
        foreach (var node in _locationLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                
                agent.Spawn(node);
                _locationLayer.GetEnvironment().Insert(agent);
                agent.Friends = new HashSet<MigrantAgent>();
                agent.Environment = environment;
                agentList.Add(agent);
                
                agent1.Spawn(node);
                _locationLayer.GetEnvironment().Insert(agent1);
                agent1.Friends = new HashSet<MigrantAgent>();
                agent1.Environment = environment;
                agentList.Add(agent1);
            }

            
        }


        var testNode = _locationLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));
      
       
        Assert.Equal(2, testNode.MigPop);

        var otherNode = _locationLayer.Entities.First(n => n.GetName().EqualsIgnoreCase("Jarablus"));

        
        //act


        agent.MoveToNode(otherNode);


        //Assert


        Assert.Equal(1, testNode.MigPop);
        Assert.Equal(1, otherNode.MigPop);
        
        
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
}