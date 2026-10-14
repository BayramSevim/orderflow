using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderFlow.Api.IntegrationTests
{
    public class OrderFlowApiTests : IClassFixture<OrderFlowApiFactory>
    {
        private const string AdminEmail = "admin@orderflow.local";
        private const string AdminPassword = "Admin12345";

        private readonly OrderFlowApiFactory _factory;

        public OrderFlowApiTests(OrderFlowApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProducts_WithoutToken_Returns401()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/products");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ApproveOrder_DecreasesStockInDatabase()
        {
            var admin = await CreateClientAsync(AdminEmail, AdminPassword);
            var dealerId = await CreateDealerAsync(admin);
            var productId = await CreateProductAsync(admin, stock: 100);

            var orderId = await PostForIdAsync(admin, "/api/orders", new
            {
                dealerId,
                items = new[] { new { productId, quantity = 30 } }
            });

            var approve = await admin.PostAsync($"/api/orders/{orderId}/approve", null);
            approve.EnsureSuccessStatusCode();

            var product = await admin.GetFromJsonAsync<ProductDto>($"/api/products/{productId}");
            Assert.Equal(70, product!.StockQuantity);
        }

        [Fact]
        public async Task Dealer_GetsOtherDealersOrder_Returns404()
        {
            var admin = await CreateClientAsync(AdminEmail, AdminPassword);
            var dealerA = await CreateDealerAsync(admin);
            var dealerB = await CreateDealerAsync(admin);
            var productId = await CreateProductAsync(admin, stock: 100);

            var orderOfA = await PostForIdAsync(admin, "/api/orders", new
            {
                dealerId = dealerA,
                items = new[] { new { productId, quantity = 5 } }
            });

            var dealerBEmail = $"b-{Guid.NewGuid():N}@test.com";
            var register = await admin.PostAsJsonAsync("/api/auth/register", new
            {
                email = dealerBEmail,
                password = "Dealer12345",
                role = "Dealer",
                dealerId = dealerB
            });
            register.EnsureSuccessStatusCode();

            var dealerBClient = await CreateClientAsync(dealerBEmail, "Dealer12345");

            var response = await dealerBClient.GetAsync($"/api/orders/{orderOfA}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ---------- helpers ----------

        private async Task<HttpClient> CreateClientAsync(string email, string password)
        {
            var client = _factory.CreateClient();

            var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
            login.EnsureSuccessStatusCode();
            var body = await login.Content.ReadFromJsonAsync<TokenDto>();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
            return client;
        }

        private static Task<Guid> CreateDealerAsync(HttpClient admin)
            => PostForIdAsync(admin, "/api/dealers", new
            {
                code = UniqueCode("D"),
                name = "Test Dealer",
                creditLimit = 10000
            });

        private static Task<Guid> CreateProductAsync(HttpClient admin, int stock)
            => PostForIdAsync(admin, "/api/products", new
            {
                code = UniqueCode("P"),
                name = "Test Product",
                price = 850,
                stockQuantity = stock
            });

        private static async Task<Guid> PostForIdAsync(HttpClient client, string url, object body)
        {
            var response = await client.PostAsJsonAsync(url, body);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IdDto>();
            return result!.Id;
        }

        private static string UniqueCode(string prefix) => $"{prefix}-{Guid.NewGuid().ToString("N")[..8]}";

        private record IdDto(Guid Id);
        private record TokenDto(string Token);
        private record ProductDto(Guid Id, int StockQuantity);
    }
}
