using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SalesManagementService.Domain.Interfaces;
using SalesManagementService.Domain.TenantSettings;

namespace SalesManagementService.Infrastructure.TenantConfig
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextcontextAccessor;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private Tenant _currentTenant;

        public TenantService(IMemoryCache cache, HttpClient httpClient, IHttpContextAccessor httpContextcontextAccessor)
        {
            _cache = cache;
            _httpClient = httpClient;
            _httpContextcontextAccessor = httpContextcontextAccessor;
            //var httpContext = _httpContextcontextAccessor.HttpContext;
            //if (httpContext == null || !httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
            //{
            //    throw new Exception("Tenant header is missing.");
            //}

            //if (!Guid.TryParse(tenantId, out var tenantGuid))
            //{
            //    throw new Exception("Invalid tenant ID format.");
            //}

            GetConnectionStringAsync();
        }

        public async Task<Tenant> GetConnectionStringAsync()
        {
            //This line is only for Migration to run.
            //return new Tenant { ConnectionString = "Server=198.38.83.24;Database=Test_DB;Port=5432;Username=postgres; Password=C@l!bre#5700", TenantId = Guid.Parse("2e2541f3-0367-44d4-8961-7684a8297590") };

            // ✅ Handle EF Core Migrations(No HTTP Context Available)
            if (_httpContextcontextAccessor.HttpContext == null)
            {
                Console.WriteLine("Running in Migration Mode: Using Default Connection String.");
                return new Tenant
                {
                    ConnectionString = "Host=198.38.83.24;Port=5432;Database=Calibre_SalesManagementDB;Username=postgres;Password=C@l!bre#5700",
                    TenantId = Guid.Parse("f46b9181-5376-443a-8629-98d1b79fe42c") // Dummy Tenant ID
                };
            }

            var httpContext = _httpContextcontextAccessor.HttpContext;
            //if (httpContext == null || !httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
            //{
            //    throw new Exception("Tenant header is missing.");
            //}

            if (httpContext == null || !httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                throw new Exception("Authorization header is missing.");
            }

            // Parse the token (Bearer <token>)
            var token = authHeader.ToString().Replace("Bearer ", string.Empty);

            // Extract tenant ID from the token
            var tenantId = ExtractTenantIdFromToken(token);

            if (!Guid.TryParse(tenantId, out var tenantGuid))
            {
                throw new Exception("Invalid tenant ID format.");
            }



            // Check if tenant configuration exists in cache
            if (_cache.TryGetValue(tenantId, out TenantConfigurations cachedTenant))
            {
                return new Tenant { ConnectionString = cachedTenant.ConnectionString, TenantId = tenantGuid };
            }

            // Call the API if not found in cache
            var response = await _httpClient.GetAsync($"/api/company-Master/{tenantId}/SalesManagementService");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve tenant configuration.");
            }

            var tenantConfig = await response.Content.ReadFromJsonAsync<TenantConfigurations>();

            if (tenantConfig == null)
            {

                throw new Exception("Tenant configuration not found.");
            }

            // Store the tenant configuration in cache
            _cache.Set(tenantId, tenantConfig, TimeSpan.FromHours(1)); // Cache for 1 hour

            //return tenantConfig.ConnectionString;

            //return tenantConfig?.ConnectionString ?? throw new Exception("Tenant connection string not found.");
            return new Tenant { ConnectionString = tenantConfig.ConnectionString, TenantId = tenantGuid };
        }

        private string ExtractTenantIdFromToken(string token)
        {
            // Decode the JWT token and extract the "tenantId" claim
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var tenantIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "TenantId");

            return tenantIdClaim?.Value;
        }
    }

    // ✅ Mock Tenant Service for Migrations
    public class MockTenantService : ITenantService
    {
        private readonly string _connectionString;

        public MockTenantService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<Tenant> GetConnectionStringAsync()
        {
            return Task.FromResult(new Tenant
            {
                ConnectionString = _connectionString,
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000000")
            });
        }
    }
}
