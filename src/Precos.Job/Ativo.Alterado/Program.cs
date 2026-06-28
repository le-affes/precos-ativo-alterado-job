namespace Ativo.Alterado;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.MapGet("/hello", () => "API .NET funcionando em v2");

        app.MapGet("/health", () => Results.Ok(new
        {
            status = "UP"
        }));

        app.Run();
    }
}