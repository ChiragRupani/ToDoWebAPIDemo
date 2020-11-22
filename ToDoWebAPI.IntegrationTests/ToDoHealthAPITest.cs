using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ToDoWebAPI.IntegrationTests
{
    public class ToDoHealthAPITest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public ToDoHealthAPITest(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Theory]
        [InlineData("/health")]
        public async Task VerifyHealthCheckAPIAsync(string url)
        {
            // Arrange
            var client = factory.CreateDefaultClient();

            // Act
            string healthStatus = await client.GetStringAsync(url);

            // Assert
            Assert.Equal("Healthy", healthStatus, true);
        }
    }
}
