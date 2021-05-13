using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoWebAPI.Models;
using Xunit;

namespace ToDoWebAPI.IntegrationTests
{
    public class ToDoAPITest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;
        
        public ToDoAPITest(CustomWebApplicationFactory<Startup> factory)
        {        
            client = factory.CreateDefaultClient();
            client.DefaultRequestVersion = HttpVersion.Version20;
        }

        [Fact]
        public async Task VerifyGetToDoAPIAsync()
        {
            // Act
            var response = await client.GetAsync("/api/todo");
            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync();
            var todos = await JsonSerializer.DeserializeAsync<List<ToDo>>(responseStream);

            // Assert            
            var expectedToDos = Utilities.getToDos();
            Assert.Equal(actual: todos, expected: expectedToDos);
        }
    }
}
