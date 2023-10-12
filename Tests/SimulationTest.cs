using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.Camps;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Location.LocationNodes;
using LaserTagBox.Model.Refugee;
using Mars.Components.Agents;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
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
  


    public SimulationTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _locationLayer = new LocationLayer();
        _locationLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData",
                    "selected_districts_for_test.geojson")
            }
        });


        _conflictLayer = new ConflictLayer();
        _conflictLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine(
                    "C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\Conflicts_Syr_Tur_2022.geojson")
            }
        });


        _campLayer = new CampLayer();
        _campLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine(
                    "C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\turkey_camps_idps.geojson")
            }
        });
        
    }

    [Fact]
    public void EnvironmentTest()
    {
        var agent = new MigrantAgent();
        LocationNode moveTestNode = _locationLayer.Entities.First();
       
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
       
       agent.MoveToNode(moveTestNode);
        MigrantAgent[] refsAtNodeAfterMoving = environment.Explore(_locationLayer.GetLocationByName("Tell Abiad").Position,
            -1D, -1, elem => elem is not null &&
            elem.Position.DistanceInKmTo(_locationLayer.GetLocationByName("Tell Abiad").Position) < 1).ToArray();
        
        
        var abstractEnvironmentObjects = environment.Explore().ToList();


        // Assert
        Assert.True(abstractEnvironmentObjects.Count == 5);
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
                _testOutputHelper.WriteLine("Neighbours: " + node.Neighbours.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 5);
                Assert.True(node.NumCamps == 0);
                Assert.True(Math.Abs(node.NormNumConflicts - (5.0 / 6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (0.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.Neighbours.Count > 0);
                
            }


            if (node.GetName().EqualsIgnoreCase("Lower Shyookh"))
            {
                _testOutputHelper.WriteLine("Camps 2: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 2: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours 2: " + node.Neighbours.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 0);
                Assert.True(node.NumCamps == 2);
                Assert.True(Math.Abs(node.NormNumConflicts - (0.0 / 6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (2.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Neighbours.Count > 0);
            }

            if (node.GetName().EqualsIgnoreCase("Abu Qalqal"))
            {
                _testOutputHelper.WriteLine("Camps 3: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 3: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours 3: " + node.Neighbours.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 0);
                Assert.True(node.NumCamps == 0);
                Assert.True(Math.Abs(node.NormNumConflicts - (0.0 / 6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (0.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.Neighbours.Count > 0);
            }

            if (node.GetName().EqualsIgnoreCase("Jarablus"))
            {
                _testOutputHelper.WriteLine("Camps 4: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 4: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours 4: " + node.Neighbours.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 4);
                Assert.True(node.NumCamps == 2);
                Assert.True(Math.Abs(node.NormNumConflicts - (4.0 / 6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (2.0 / 3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.Neighbours.Count > 0);
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


        LocationNode? updateNormRefPopTestNode = _locationLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));

        var environment = _locationLayer.GetEnvironment();


        // Act

        var calculatedMaxRefPop = _locationLayer.MaxRefPop();
        var agents = environment.Explore(new Position(), -1D, -1, a => a != null);
        var environmentObjects = agents.ToList();
        updateNormRefPopTestNode.UpdateNormRefPop(calculatedMaxRefPop);

        //Assert
        _testOutputHelper.WriteLine(environmentObjects.Count.ToString());
        Assert.True(environmentObjects.ToList().Count == 5);
        Assert.Equal(2, calculatedMaxRefPop);
        Assert.Equal(1.0, updateNormRefPopTestNode.NormRefPop);
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
        testNode.GetRandomRefugeesAtNode(environment);

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


        testNode.GetRandomRefugeesAtNode(environment);
        int calcNumFriendsAtNode = agent.GetNumFriendsAtNode(testNode);
        int calcNumFriendsAtNode1 = agent1.GetNumFriendsAtNode(testNode);


        //Assert


        Assert.Equal(1, calcNumFriendsAtNode);
        Assert.Equal(1, calcNumFriendsAtNode1);
    }


    [Fact]
    public void TurkeyDistrictsNeighboursTest()
    {
        // Arrange
        var turkeyNodeLayer = new LocationLayer();
        turkeyNodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\turkey_districts_2.geojson")
            }
        });

        turkeyNodeLayer.StartMonth = 9;

        // Act

        foreach (var node in turkeyNodeLayer.Entities)
        {
            node.InitConflicts(_conflictLayer);
            node.InitCamps(_campLayer);
        }

        turkeyNodeLayer.InitLocationParams();
        turkeyNodeLayer.PreTick();


        //Assert

        foreach (var node in turkeyNodeLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("CEYLANPINAR"))
            {
                _testOutputHelper.WriteLine("Node: " + node.GetName());
                _testOutputHelper.WriteLine("Neighbours: " + node.Neighbours.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.Equal(5, node.Neighbours.Count);
                foreach (var nodeNeighbour in node.Neighbours)
                {
                    Assert.NotNull(nodeNeighbour);
                }
            }
            if (node.GetName().EqualsIgnoreCase("KIRIKHAN"))
            {
                _testOutputHelper.WriteLine("Node: " + node.GetName());
                _testOutputHelper.WriteLine("Neighbours: " + node.Neighbours.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.Equal(5, node.Neighbours.Count);
                foreach (var nodeNeighbour in node.Neighbours)
                {
                    Assert.NotNull(nodeNeighbour);
                }
            }
           

            if (node.GetName().EqualsIgnoreCase("SILIFKE"))
            {
                _testOutputHelper.WriteLine("Node: " + node.GetName());
                _testOutputHelper.WriteLine("Neighbours: " + node.Neighbours.Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.Equal(5, node.Neighbours.Count);
                foreach (var nodeNeighbour in node.Neighbours)
                {
                    Assert.NotNull(nodeNeighbour);
                }
            }

            
        }
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
      
       
        Assert.Equal(2, testNode.RefPop);

        var otherNode = _locationLayer.Entities.First(n => n.GetName().EqualsIgnoreCase("Jarablus"));

        
        //act


        agent.MoveToNode(otherNode);


        //Assert


        Assert.Equal(1, testNode.RefPop);
        Assert.Equal(1, otherNode.RefPop);
        
        
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
}