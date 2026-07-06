using Ativo.Alterado.Service;
using Microsoft.Extensions.DependencyInjection;
using static Ativo.Alterado.Infrastrucuture.WebApplicationBuilder.BuildHost;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = GetBuilder(args);

        using var host = builder.Build();

        using var scope = host.Services.CreateScope();

        var sqsService = scope.ServiceProvider.GetRequiredService<SqsService>();

        // chama seu método aqui
        // sqsService.Processar();
    }
}