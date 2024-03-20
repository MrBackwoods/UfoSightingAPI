using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;
using UfoSightingAPI.Models;

public class ValidatePermissionsFilter : ActionFilterAttribute
{
    private UfoSightingDBContext? _dbContext;
    public const string _apiKeyKey = "ApiKey";
    public const string _memberIDKey = "MemberID";

    public bool NeedsAdminRights = false;
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Get the DB context from the request services
        _dbContext = context.HttpContext.RequestServices.GetRequiredService<UfoSightingDBContext>();

        // If the DB context is null, return a 400 Bad Request response
        if (_dbContext == null || !_dbContext.Database.CanConnect())
        {
            context.Result = CreateServerErrorResult();
            return;
        }

        // Check if the request has an API key, if not return a 403 Forbidden response
        if (!context.HttpContext.Request.Headers.ContainsKey(_apiKeyKey))
        {
            context.Result = CreateForbiddenResult();
            return;
        }

        // Retrieve API key from request headers 
        string apiKey = context.HttpContext.Request.Headers[_apiKeyKey].FirstOrDefault() ?? "";

        // If the API key is null, return a 403 Forbidden response
        if (string.IsNullOrEmpty(apiKey))
        {
            context.Result = CreateForbiddenResult();
            return;
        }

        Member? member = ValidateAPIKey(apiKey);

        // If the API key is not valid, return a 403 Forbidden response
        if (member == null)
        {
            context.Result = CreateForbiddenResult();
            return;
        }

        // If the member is not an admin and the action requires admin rights, return a Forbidden response
        if (member.IsAdmin != true && NeedsAdminRights)
        {
            context.Result = CreateForbiddenResult();
            return;
        }

        // Add the resolved member ID to the request context before the action is executed
        context.HttpContext.Items.Add(_memberIDKey, member.MemberId);

        base.OnActionExecuting(context);
    }

    // Validate the API key
    public Member? ValidateAPIKey(string? apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) return null;

        // Get today
        DateTime dateTimeToday = DateTime.Today;
        DateOnly dateOnlyToday = new DateOnly(dateTimeToday.Year, dateTimeToday.Month, dateTimeToday.Day);
        return _dbContext?.Member.Where(m => m.ApiKey == HashApiKey(apiKey) && m.ApiKeyActivationDate <= dateOnlyToday && m.ApiKeyDeactivationDate >= dateOnlyToday).SingleOrDefault();
    }

    // Hash the API key
    private static string HashApiKey(string? apiKey)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.Unicode.GetBytes(apiKey);
            byte[] hashedBytes = sha256.ComputeHash(inputBytes);
            string hashed = Convert.ToBase64String(hashedBytes);
            return hashed;
        }
    }

    // Create a 403 Forbidden response
    public ContentResult CreateForbiddenResult()
    {
        return new ContentResult
        {
            Content = "Access denied.",
            ContentType = "text/plain",
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    // Create a 500 Error response
    public ContentResult CreateServerErrorResult()
    {
        return new ContentResult
        {
            Content = "Cannot connect to database.",
            ContentType = "text/plain",
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
