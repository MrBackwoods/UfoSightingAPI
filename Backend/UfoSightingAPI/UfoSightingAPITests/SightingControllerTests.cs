using UfoSightingAPI.Controllers;
using UfoSightingAPI.Models;
using Microsoft.AspNetCore.Http;
using UfoSightingAPITests;

public class SightingControllerTests
{
    // Test getting all sightings with limited information
    [Fact]
    public async Task GetSightingsLimited()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var controller = new SightingController(dbContext);

        var actionResult = await controller.GetSightingsLimited(DateTime.Today.Year);
        var sightings = actionResult.Value as IEnumerable<Sighting>;
        Assert.Equal(1, sightings?.Count());
        Assert.Null(sightings?.First().Description);
    }

    // Test deleting sighting
    [Fact]
    public async Task DeleteSighting()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var controller = new SightingController(dbContext);

        await controller.DeleteSighting(2);
        var actionResult = await controller.GetSightingsLimited(DateTime.Today.Year);
        var sightings = actionResult.Value as IEnumerable<Sighting>;

        Assert.Equal(0, sightings?.Count());
    }

    // Test getting a sighting wit full information
    [Fact]
    public async Task GetSighting()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var controller = new SightingController(dbContext);

        var actionResult = await controller.GetSightingWithDetails(1);
        var sighting = actionResult.Value as Sighting;

        Assert.Equal(1, sighting?.SightingId);
        Assert.NotNull(sighting?.Description);
    }

    // Test posting a sighting
    [Fact]
    public async Task PostSighting()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var controller = new SightingController(dbContext);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Items[ValidatePermissionsFilter._memberIDKey] = 1;

        var actionResult = await controller.PostSighting(new Sighting { Occurred = DateTime.Today, Latitude = (decimal)47.0, Longitude = (decimal)47.0, Description = "test" });
        var actionResult2 = await controller.GetSightingsLimited(DateTime.Today.Year);
        var sightings = actionResult2.Value as IEnumerable<Sighting>;

        Assert.Equal(2, sightings?.Count());
    }

    // Test posting a sighting with sightingId
    [Fact]
    public async Task PostSightingWithSightingId()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var controller = new SightingController(dbContext);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Items[ValidatePermissionsFilter._memberIDKey] = 1;

        var actionResult = await controller.PostSighting(new Sighting { SightingId = 1, Occurred = DateTime.Today, Latitude = (decimal)47.0, Longitude = (decimal)47.0, Description = "test" });
        var actionResult2 = await controller.GetSightingsLimited(DateTime.Today.Year);
        var sightings = actionResult2.Value as IEnumerable<Sighting>;

        Assert.Equal(2, sightings?.Count());

        foreach (var sighting in sightings)
        {
            Assert.NotEqual(0, sighting.SightingId);
        };
    }
}
