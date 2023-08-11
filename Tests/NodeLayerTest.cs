using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    private NodeLayer nodeLayer;
    private ConflictLayer conflictLayer = new ConflictLayer();


    private CampLayer campLayer;




    public NodeLayerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        nodeLayer = new NodeLayer();
        nodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "selected_districts_for_test.geojson")
            }
        });



        conflictLayer = new ConflictLayer();
        conflictLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\Conflicts_Syr_Tur_2022.geojson")
            }
        });
        
        
        campLayer = new CampLayer();
        campLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\RefugeeSimulation\\Resources\\turkey_camps_idps.geojson")
            }
        });
    }

    [Fact]
    public void EnvironmentTest()
    {
        
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

        nodeLayer.StartMonth = 9;
        
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
                Assert.True(node.GetNeighbours().Count> 0);
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
                Assert.True(node.GetNeighbours().Count > 0);
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
                Assert.True(node.GetNeighbours().Count > 0);
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
                Assert.True(node.GetNeighbours().Count > 0);
            }
        }

    }


    [Fact]
    public void PreTickTest()
    {
        //Arrange
        
     
        
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
        
    


        var agent = new RefugeeAgent();
        var agent1 = new RefugeeAgent();
        var environment = nodeLayer.GetEnvironment();
        var agentList = new List<RefugeeAgent>();
        foreach(var node in nodeLayer.Entities)
        {

            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                agent = new RefugeeAgent();
                agent.Spawn(node);
                nodeLayer.GetEnvironment().Insert(agent);
                agent.Friends = new HashSet<RefugeeAgent>();
                agent.Environment = environment;
                agentList.Add(agent);
            }
            agent1 = new RefugeeAgent();
            agent1.Spawn(node);
            nodeLayer.GetEnvironment().Insert(agent1);
            agent1.Friends = new HashSet<RefugeeAgent>();
            agent1.Environment = environment;
            agentList.Add(agent1);

        }
        
        
        var testNode = nodeLayer.Entities
            .First(n => n.GetName().EqualsIgnoreCase("Tell Abiad"));
        nodeLayer.MaxRefPop();
        
        
        //act
        
        
        
        testNode.GetRandomRefugeesAtNode(environment);
        int calcNumFriendsAtNode = agent.GetNumFriendsAtNode(testNode, agentList);
        int calcNumFriendsAtNode1 = agent1.GetNumFriendsAtNode(testNode, agentList);
        
        
        //Assert
        
        
        Assert.Equal(1,calcNumFriendsAtNode);
        Assert.Equal(1, calcNumFriendsAtNode1);
    }


    [Fact]
    public void TurkeyDistrictsNeighboursTest()
    {
        // Arrange
        var turkeyNodeLayer = new NodeLayer();
        turkeyNodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "turkey_districts_test.geojson")
            }
        });
        
        turkeyNodeLayer.StartMonth = 9;
        
        // Act

        foreach (var node in nodeLayer.Entities)
        {
            node.InitConflicts(conflictLayer);
            node.InitCamps(campLayer);
        }

        nodeLayer.InitLocationParams();
        nodeLayer.PreTick();
        
       
        
        
        
        //Assert

        foreach (var node in turkeyNodeLayer.Entities)
        {
            if (node.GetName().EqualsIgnoreCase("SANLIURFA"))
            {
                _testOutputHelper.WriteLine("Node: " + node.GetName());
                _testOutputHelper.WriteLine("Camps: " + node.GetNumCampsAtNode());
                _testOutputHelper.WriteLine("Conflicts: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.Equal(3,node.GetNeighbours().Count);
            }
            
            if (node.GetName().EqualsIgnoreCase("MARDIN"))
            {
                _testOutputHelper.WriteLine("Node: " + node.GetName());
                _testOutputHelper.WriteLine("Camps: " + node.GetNumCampsAtNode());
                _testOutputHelper.WriteLine("Conflicts: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.Equal(3,node.GetNeighbours().Count);
            }
            
            if (node.GetName().EqualsIgnoreCase("KILIS"))
            {
                _testOutputHelper.WriteLine("Node: " + node.GetName());
                _testOutputHelper.WriteLine("Camps: " + node.GetNumCampsAtNode());
                _testOutputHelper.WriteLine("Conflicts: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.Equal(3,node.GetNeighbours().Count);
            }
            
            if (node.GetName().EqualsIgnoreCase("HATAY"))
            {
                _testOutputHelper.WriteLine("Node: " + node.GetName());
                _testOutputHelper.WriteLine("Camps: " + node.GetNumCampsAtNode());
                _testOutputHelper.WriteLine("Conflicts: " + node.NumConflicts);
                _testOutputHelper.WriteLine("Neighbours: " + node.GetNeighbours().Count);
                _testOutputHelper.WriteLine("---------------------------------------------");
                Assert.True(node.GetNeighbours().Count>2 && node.GetNeighbours().Count<5);
            }
        }
    }
}