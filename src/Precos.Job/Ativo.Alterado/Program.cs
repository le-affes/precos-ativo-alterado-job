using Amazon;
using Amazon.SQS;
using Ativo.Alterado.Services;

namespace Ativo.Alterado;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION")
                        ?? throw new InvalidOperationException("Variável de ambiente AWS_REGION não configurada.");

        builder.Services.AddSingleton<IAmazonSQS>(_ =>
        {
            return new AmazonSQSClient(RegionEndpoint.GetBySystemName(awsRegion));
        });

        builder.Services.AddScoped<SqsPublisherService>();

        var app = builder.Build();

        app.MapGet("/hello", () => "API .NET funcionando em v2");

        app.MapGet("/health", () => Results.Ok(new
        {
            status = "UP"
        }));

        app.MapGet("/sqs/teste-publicacao", async (SqsPublisherService sqsPublisherService) =>
        {
            var messageId = await sqsPublisherService.EnviarMensagemTesteAsync();

            return Results.Ok(new
            {
                sucesso = true,
                fila = "operacao-registrada.fifo",
                messageId
            });
        });

        app.Run();
    }
}