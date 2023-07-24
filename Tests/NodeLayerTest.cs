using System.IO;
using LaserTagBox.Model.Location.LocationNodes;
using Mars.Interfaces.Data;
using Xunit;

namespace Tests;

/* Test that locations and agents are accessible in Environment
   Test that location params are initialized correctly
   Test that CalcScore works properly
   Test that MaxRefPop works properly
hgh*/
public class NodeLayerTest
{
    [Fact]
    public void Test1()
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

        // A

        // Assert
    }
}