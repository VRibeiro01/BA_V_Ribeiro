using System.IO;
using LaserTagBox.Model.Location.LocationNodes;
using Mars.Interfaces.Data;
using Xunit;

namespace Tests;

/* Test that conflicts and camps are initialized correctly
   Test UpdateNormRefPop works correctly
   Test that GetRandomRefugeesAtNode works correctly
hgh*/
public class UnitTest1
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
                File = Path.Combine("RefugeeSimulation/Resources", "syrian_districts_2.geojson")
            }
        }, null, null);

        // Act

        // Assert
    }
}