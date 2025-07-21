using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthConditionForecast.Tests
{
    public static class TestClaimsPrincipalFactory
    {
        public static ClaimsPrincipal Create(string role)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "test-user-id"),
            new Claim(ClaimTypes.Role, role)
        }, "mock"));
        }
    }

}
