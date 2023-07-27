using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.Camps;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Location.LocationNodes;
using LaserTagBox.Model.Refugee;
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
public class NodeLayerTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public NodeLayerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void EnvironmentTest()
    {
        
        // Arrange
        var nodeLayer = new NodeLayer();
        nodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "selected_districts_for_test.geojson")
            }
        });

       
       
        foreach(var node in nodeLayer.Entities)
        {

            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                var agent = new RefugeeAgent();
                agent.Spawn(node);
                nodeLayer.GetEnvironment().Insert(agent);
            }
            var agent1 = new RefugeeAgent();
            agent1.Spawn(node);
            nodeLayer.GetEnvironment().Insert(agent1);
            
            
        }


        // Act
      
        var environment = nodeLayer.GetEnvironment();
        
        var agents = environment.Explore();
        var abstractEnvironmentObjects = agents.ToList();

       
        

        // Assert
        Assert.True(abstractEnvironmentObjects.Count == 5);
        
       
    }

    [Fact]
    public void LocationInitializationTest()
    {
        // Arrange
     
        var nodeLayer = new NodeLayer();
        nodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "selected_districts_for_test.geojson")
            }
        });
        
     
        var conflictLayer = new ConflictLayer();
        conflictLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\Conflicts_Syr_Tur_0922.geojson")
            }
        });
        
        var campLayer = new CampLayer();
        campLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\turkey_camps_idps.geojson")
            }
        });
        
        
        // Act

        foreach (var node in nodeLayer.Entities)
        {
            node.InitConflicts(conflictLayer);
            node.InitCamps(campLayer);
        }

        nodeLayer.InitLocationParams();
        nodeLayer.PreTick();
        
       
        
        
        
        //Assert

        foreach (var node in nodeLayer.Entities)
        {
            if(node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                _testOutputHelper.WriteLine("Camps: " + node.GetNumCampsAtNode());
                _testOutputHelper.WriteLine("Conflicts: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 5);
                Assert.True(node.NumCamps == 0);
                Assert.True(Math.Abs(node.NormNumConflicts - (5.0/6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (0.0/3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.GetNeighbours().Count == 0);
            }
            
            
            if(node.GetName().EqualsIgnoreCase("Lower Shyookh"))
            {
                _testOutputHelper.WriteLine("Camps 2: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 2: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours 2: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 0);
                Assert.True(node.NumCamps == 2);
                Assert.True(Math.Abs(node.NormNumConflicts - (0.0/6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (2.0/3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.GetNeighbours().Count == 1);
            }
            
            if(node.GetName().EqualsIgnoreCase("Abu Qalqal"))
            {
                _testOutputHelper.WriteLine("Camps 3: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 3: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours 3: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 0);
                Assert.True(node.NumCamps == 0);
                Assert.True(Math.Abs(node.NormNumConflicts - (0.0/6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (0.0/3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.GetNeighbours().Count == 0);
            }
            
            if(node.GetName().EqualsIgnoreCase("Jarablus"))
            {
                _testOutputHelper.WriteLine("Camps 4: " + node.NumCamps);
                _testOutputHelper.WriteLine("Conflicts 4: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours 4: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.NumConflicts == 4);
                Assert.True(node.NumCamps == 2);
                Assert.True(Math.Abs(node.NormNumConflicts - (4.0/6.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (2.0/3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                Assert.True(node.GetNeighbours().Count == 1);
            }
        }

    }


    [Fact]
    public void PreTickTest()
    {
        //Arrange
        
        
        var nodeLayer = new NodeLayer();
        nodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "selected_districts_for_test.geojson")
            }
        });


        
        foreach(var node in nodeLayer.Entities)
        {

            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                var agent = new RefugeeAgent();
                agent.Spawn(node);
                nodeLayer.GetEnvironment().Insert(agent);
            }
            var agent1 = new RefugeeAgent();
            agent1.Spawn(node);
            nodeLayer.GetEnvironment().Insert(agent1);
            
            
        }


        LocationNode? updateNormRefPopTestNode = nodeLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));
        
        var environment = nodeLayer.GetEnvironment();
        

        // Act

        var calculatedMaxRefPop = nodeLayer.MaxRefPop();
        var agents = environment.Explore(new Position(), -1D, -1, a => a is RefugeeAgent);
        var environmentObjects = agents.ToList();
        updateNormRefPopTestNode.UpdateNormRefPop(calculatedMaxRefPop);
        
        //Assert
        _testOutputHelper.WriteLine(environmentObjects.Count.ToString());
        Assert.True(environmentObjects.ToList().Count == 5);
        Assert.DoesNotContain(environmentObjects, a => a is LocationNode);
        Assert.Equal(2,calculatedMaxRefPop);
        Assert.DoesNotContain(environmentObjects, a => a is LocationNode);
        Assert.Equal(1.0, updateNormRefPopTestNode.NormRefPop);
        

        
    }

    [Fact]
    public void UpdateSocialNetworkTest()
    {
        //Arrange
        
        
        var nodeLayer = new NodeLayer();
        nodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "selected_districts_for_test.geojson")
            }
        });


        var agent = new RefugeeAgent();
        var agent1 = new RefugeeAgent();
        foreach(var node in nodeLayer.Entities)
        {

            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                agent = new RefugeeAgent();
                agent.Spawn(node);
                nodeLayer.GetEnvironment().Insert(agent);
                agent.Friends = new HashSet<RefugeeAgent>();
            }
            agent1 = new RefugeeAgent();
            agent1.Spawn(node);
            nodeLayer.GetEnvironment().Insert(agent1);
            agent1.Friends = new HashSet<RefugeeAgent>();
            
        }

       
       
        
        var environment = nodeLayer.GetEnvironment();
        var testNode = nodeLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));
        
        
        //act
        nodeLayer.MaxRefPop();
        testNode.GetRandomRefugeesAtNode(environment);
        
        //Assert
        
        Assert.Contains(environment.Explore().Select(e => (RefugeeAgent)e), e => !e.Friends.IsEmpty());
        
    }


    [Fact]
    public void RefugeeNumContactsAtNode()
    {
        //Arrange
        
        
        var nodeLayer = new NodeLayer();
        nodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "selected_districts_for_test.geojson")
            }
        });


        var agent = new RefugeeAgent();
        var agent1 = new RefugeeAgent();
        var environment = nodeLayer.GetEnvironment();
        foreach(var node in nodeLayer.Entities)
        {

            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                agent = new RefugeeAgent();
                agent.Spawn(node);
                nodeLayer.GetEnvironment().Insert(agent);
                agent.Friends = new HashSet<RefugeeAgent>();
                agent.Environment = environment;
            }
            agent1 = new RefugeeAgent();
            agent1.Spawn(node);
            nodeLayer.GetEnvironment().Insert(agent1);
            agent1.Friends = new HashSet<RefugeeAgent>();
            agent1.Environment = environment;

        }
        
        
        var testNode = nodeLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));
        nodeLayer.MaxRefPop();
        
        
        //act
        
        
        
        testNode.GetRandomRefugeesAtNode(environment);
        int calcNumFriendsAtNode = agent.GetNumFriendsAtNode(testNode);
        int calcNumFriendsAtNode1 = agent1.GetNumFriendsAtNode(testNode);
        
        
        //Assert
        
        
        Assert.Equal(1,calcNumFriendsAtNode);
        Assert.Equal(1, calcNumFriendsAtNode1);
    }
}