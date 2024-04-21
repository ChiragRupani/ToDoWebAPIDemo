namespace ToDoWebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        Startup startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        WebApplication app = builder.Build();
        startup.Configure(app, app.Environment);
        app.Run();
    }
}
