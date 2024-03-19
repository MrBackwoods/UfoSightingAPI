using Microsoft.EntityFrameworkCore;
using UfoSightingAPI.Controllers;
using UfoSightingAPI.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

public class SightingControllerTests
{
    private UfoSightingDBContext GetContextWithData()
    {
        var options = new DbContextOptionsBuilder<UfoSightingDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new UfoSightingDBContext(options);

        context.Sighting.AddRange(
            new Sighting { SightingId = 1, Occurred = DateTime.Today.AddYears(-1), Latitude = (decimal)45.0, Longitude = (decimal)45.0, Description = "test" },
            new Sighting { SightingId = 2, Occurred = DateTime.Today, Latitude = (decimal)46.0, Longitude = (decimal)46.0, Description = "test" }
        );

        context.Member.AddRange(
            new Member { MemberId = 1, ApiKey = "MyRhy0ZUmHpWJL+Q5FYTy7SD58RVJwJV1QjWlMstfX0=", FirstName = "A", LastName = "B", Email ="A@B.fi" }
        ) ;

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetSightingsLimited()
    {
        using var context = GetContextWithData();
        var controller = new SightingController(context);
        var actionResult = await controller.GetSightingsLimited(DateTime.Today.Year);
        var sightings = actionResult.Value as IEnumerable<Sighting>;
        Assert.Equal(1, sightings?.Count());
        Assert.Null(sightings?.First().Description);
    }

    [Fact]
    public async Task DeleteSighting()
    {
        using var context = GetContextWithData();
        var controller = new SightingController(context);
        await controller.DeleteSighting(2);
        var actionResult = await controller.GetSightingsLimited(DateTime.Today.Year);
        var sightings = actionResult.Value as IEnumerable<Sighting>;
        Assert.Equal(0, sightings?.Count());
    }

    [Fact]
    public async Task GetSighting()
    {
        using var context = GetContextWithData();
        var controller = new SightingController(context);
        var actionResult = await controller.GetSightingWithDetails(1);
        var sighting = actionResult.Value as Sighting;
        Assert.Equal(1, sighting?.SightingId);
        Assert.NotNull(sighting?.Description);
    }

    [Fact]
    public async Task PostSighting()
    {
        using var context = GetContextWithData();
        var controller = new SightingController(context);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Items[ValidatePermissions._memberIDKey] = 1;
        var actionResult = await controller.PostSighting(new Sighting { Occurred = DateTime.Today, Latitude = (decimal)47.0, Longitude = (decimal)47.0, Description = "test" });
        var actionResult2 = await controller.GetSightingsLimited(DateTime.Today.Year);
        var sightings = actionResult2.Value as IEnumerable<Sighting>;
        Assert.Equal(2, sightings?.Count());
    }

    [Fact]
    public void ValidatePermissions_ApiKey_Is_Missing()
    {
        using var dbContext = GetContextWithData();

        var context = new ActionExecutingContext(
            new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            ),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            null);

        var filter = new ValidatePermissions()
        {
            NeedsAdminRights = true
        };

        var services = new ServiceCollection();
        services.AddSingleton<UfoSightingDBContext>(dbContext);
        context.HttpContext.RequestServices = services.BuildServiceProvider();
        filter.OnActionExecuting(context);
        Assert.IsType<ContentResult>(context.Result);
        var contentResult = context.Result as ContentResult;
        Assert.Equal("Access denied.", contentResult?.Content);
        Assert.Equal(StatusCodes.Status403Forbidden, contentResult?.StatusCode);
    }

    [Fact]
    public void ValidatePermissions_ApiKey_Is_Valid()
    {
        using var dbContext = GetContextWithData();

        var context = new ActionExecutingContext(
            new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            ),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            null);

        var filter = new ValidatePermissions()
        {
            NeedsAdminRights = false
        };

        var services = new ServiceCollection();
        services.AddSingleton<UfoSightingDBContext>(dbContext);
        context.HttpContext.RequestServices = services.BuildServiceProvider();
        context?.HttpContext.Request.Headers.Add(ValidatePermissions._apiKeyKey, "dummy-key-2");
        filter.OnActionExecuting(context);
        var contentResult = context.Result as ContentResult;
        Assert.Null(context.Result);
    }
}
