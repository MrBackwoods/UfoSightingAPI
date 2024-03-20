using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UfoSightingAPI.Models;

namespace UfoSightingAPITests
{
    public static class TestContextBuilder
    {
        public static UfoSightingDBContext GetDBContextWithData()
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
                new Member { MemberId = 1, ApiKey = "MyRhy0ZUmHpWJL+Q5FYTy7SD58RVJwJV1QjWlMstfX0=", FirstName = "A", LastName = "B", Email = "A@B.fi" }
            );

            context.SaveChanges();
            return context;
        }

        public static ActionExecutingContext GetActionExecutingContext()
        {
            return new ActionExecutingContext(
                    new ActionContext(
                        new DefaultHttpContext(),
                        new Microsoft.AspNetCore.Routing.RouteData(),
                        new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                null);
        }
    }
}
