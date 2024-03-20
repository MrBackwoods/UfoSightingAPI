using Microsoft.EntityFrameworkCore;
using UfoSightingAPI.Controllers;
using UfoSightingAPI.Models;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UfoSightingAPITests;

public class ValidatePermissionsTests
{
    [Fact]
    // Test that the filter returns 403 when the API key is missing
    public void ValidatePermissions_ApiKey_Is_Missing()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var actionContext = TestContextBuilder.GetActionExecutingContext();

        var filter = new ValidatePermissionsFilter()
        {
            NeedsAdminRights = true
        };

        var services = new ServiceCollection();
        services.AddSingleton<UfoSightingDBContext>(dbContext);
        actionContext.HttpContext.RequestServices = services.BuildServiceProvider();
        filter.OnActionExecuting(actionContext);
        Assert.IsType<ContentResult>(actionContext.Result);
        var contentResult = actionContext.Result as ContentResult;
        Assert.Equal("Access denied.", contentResult?.Content);
        Assert.Equal(StatusCodes.Status403Forbidden, contentResult?.StatusCode);
    }

    [Fact]
    // Test that the filter returns null when API key is valid - means that the request is allowed to proceed
    public void ValidatePermissions_ApiKey_Is_Valid()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var actionContext = TestContextBuilder.GetActionExecutingContext();

        var filter = new ValidatePermissionsFilter()
        {
            NeedsAdminRights = false
        };

        var services = new ServiceCollection();
        services.AddSingleton<UfoSightingDBContext>(dbContext);
        actionContext.HttpContext.RequestServices = services.BuildServiceProvider();
        actionContext?.HttpContext.Request.Headers.Append(ValidatePermissionsFilter._apiKeyKey, "dummy-key-2");
        filter.OnActionExecuting(actionContext);
        var contentResult = actionContext.Result as ContentResult;
        Assert.Null(actionContext.Result);
    }

    [Fact]
    // Test that the filter returns null when API key is valid but action requires admin rights
    public void ValidatePermissions_Admin_Check_Fail()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var actionContext = TestContextBuilder.GetActionExecutingContext();

        var filter = new ValidatePermissionsFilter()
        {
            NeedsAdminRights = true
        };

        var services = new ServiceCollection();
        services.AddSingleton<UfoSightingDBContext>(dbContext);
        actionContext.HttpContext.RequestServices = services.BuildServiceProvider();
        actionContext?.HttpContext.Request.Headers.Append(ValidatePermissionsFilter._apiKeyKey, "dummy-key-2");
        filter.OnActionExecuting(actionContext);
        var contentResult = actionContext.Result as ContentResult;
        Assert.Equal("Access denied.", contentResult?.Content);
        Assert.Equal(StatusCodes.Status403Forbidden, contentResult?.StatusCode);
    }

    [Fact]
    // Test that the filter returns 403 when the API key is wrong
    public void ValidatePermissions_ApiKey_Is_Wrong()
    {
        using var dbContext = TestContextBuilder.GetDBContextWithData();
        var actionContext = TestContextBuilder.GetActionExecutingContext();

        var filter = new ValidatePermissionsFilter()
        {
            NeedsAdminRights = false
        };

        var services = new ServiceCollection();
        services.AddSingleton<UfoSightingDBContext>(dbContext);
        actionContext.HttpContext.RequestServices = services.BuildServiceProvider();
        actionContext?.HttpContext.Request.Headers.Append(ValidatePermissionsFilter._apiKeyKey, "dummy-key-3");
        filter.OnActionExecuting(actionContext) ;
        var contentResult = actionContext.Result as ContentResult;
        Assert.Equal("Access denied.", contentResult?.Content);
        Assert.Equal(StatusCodes.Status403Forbidden, contentResult?.StatusCode);
    }
}
