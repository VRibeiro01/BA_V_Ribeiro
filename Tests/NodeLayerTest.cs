using System;
using System.IO;
using System.Linq;
using LaserTagBox.Model.Location.Conflict;
using LaserTagBox.Model.Location.LocationNodes;
using LaserTagBox.Model.Refugee;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using ServiceStack;
using Xunit;

namespace Tests;

/* Test that locations and agents(agents --> move to ref test) are accessible in Environment
   Test that location params are initialized correctly
   Test that CalcScore works properly
   Test that MaxRefPop works properly --> Move to ref test
hgh*/
public class NodeLayerTest
{
    [Fact]
    public void ExploreLocationsInEnvironmentTest()
    {
        
        // Arrange
        var nodeLayer = new NodeLayer();
        nodeLayer.InitLayer(new LayerInitData
        {
            LayerInitConfig =
            {
                File = Path.Combine("C:\\Users\\vivia\\mars\\RefugeeSimulationSolution\\Tests\\TestData", "selected_districts_for_test.geojson")
            }
        }, null, null);


       

        // Act
        nodeLayer.InsertLocationsInEnvironment();
        var environment = nodeLayer.GetEnvironment();
        var locations = environment.Explore(new Position(), -1D, -1, e => e is LocationNode);
        var abstractEnvironmentObjects = locations.ToList();

        // Assert
        Assert.DoesNotContain(abstractEnvironmentObjects.ToList(), e => e is RefugeeAgent);
        Assert.NotEmpty(abstractEnvironmentObjects);
        Assert.True(abstractEnvironmentObjects.All(e=> e is LocationNode));
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
        }, null, null);
        
     
        
        
        // Act
        
        var nodesList = nodeLayer.Entities.ToList();
        nodeLayer.Entities.FirstOrDefault(n => n.GetName().EqualsIgnoreCase("Tell Abiad"))!.NumConflicts = 4;
        nodeLayer.Entities.FirstOrDefault(n => n.GetName().EqualsIgnoreCase("Tell Abiad"))!.NumCamps = 2;
        nodeLayer.InitLocationParams();
        nodeLayer.PreTick();
        
        
        //Assert

        foreach (var node in nodesList)
        {
            if (node.GetName().EqualsIgnoreCase("Tell Abiad"))
            {
                Assert.True(Math.Abs(node.NormNumConflicts - (4.0/5.0)) < 0.01);
                Assert.True(Math.Abs(node.NormNumCamps - (2.0/3.0)) < 0.01);
                Assert.True(node.AnchorScore > 0);
                Assert.True(node.NormAnchorScore > 0);
                Assert.True(node.Score != 0);
                
            }
            Assert.NotEmpty(node.Neighbours);
            
        }
    }
}