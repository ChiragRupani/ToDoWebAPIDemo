using System.Net;
using ToDoWebAPI.Models;
using Xunit;

namespace ToDoWebAPI.IntegrationTests;

public class ToDoAPITest : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient client;

    public ToDoAPITest(CustomWebApplicationFactory<Startup> factory)
    {
        client = factory.CreateClient();
        client.DefaultRequestVersion = HttpVersion.Version30;
    }

    [Fact]
    public async Task VerifyGetToDoAPIAsync()
    {
        // Act        
        var todos = await client.GetFromJsonAsync<List<ToDo>>("/api/todo", cancellationToken: TestContext.Current.CancellationToken);

        // Assert            
        var expectedToDos = Utilities.GetToDos();
        Assert.Equal(actual: todos, expected: expectedToDos);
    }

    [Fact]
    public async Task VerifyGetSingleToDoAPIAsync()
    {
        // Act        
        var todo = await client.GetFromJsonAsync<ToDo>("/api/todo/1", cancellationToken: TestContext.Current.CancellationToken);

        // Assert            
        var expectedToDos = Utilities.GetToDos();
        Assert.Equal(actual: todo, expected: expectedToDos[0]);
    }
}
