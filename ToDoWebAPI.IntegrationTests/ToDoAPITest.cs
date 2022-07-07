using System.Net;
using System.Text.Json;
using ToDoWebAPI.Models;
using Xunit;

namespace ToDoWebAPI.IntegrationTests;

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
        var todos = await client.GetFromJsonAsync<List<ToDo>>("/api/todo");

        // Assert            
        var expectedToDos = Utilities.GetToDos();
        Assert.Equal(actual: todos, expected: expectedToDos);
    }
}
