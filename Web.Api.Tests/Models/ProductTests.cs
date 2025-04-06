using Web.Api.Models;

namespace Web.Api.Tests.Models;
public class ProductTests
{
    [Fact]
    public void Constructor_GivenAllParameters_ThenShouldSetThePropertiesCorrectly()
    {
        //Arrange
        var expectedId = Guid.NewGuid();
        var expectedName = "Test product";
        var expectedPrice = 200.00m;
        var expectedDescription = "Test description";

        //Act
        var product = new Product(expectedId, expectedName, expectedPrice, expectedDescription);

        //Assert
        Assert.Equal(expectedId, product.Id);
        Assert.Equal(expectedName, product.Name);
        Assert.Equal(expectedPrice, product.Price);
        Assert.Equal(expectedDescription, product.Description);
    }
}