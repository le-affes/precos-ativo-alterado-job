using Amazon;
using Amazon.SQS;
using Ativo.Alterado.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System.Data;
using static Ativo.Alterado.Infrastrucuture.WebApplicationBuilder.ConnectionString;

namespace Ativo.Alterado.Infrastrucuture.WebApplicationBuilder;

public static class BuildHost
{
    public static HostApplicationBuilder GetBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION")
            ?? throw new InvalidOperationException(
                "Variável de ambiente AWS_REGION não configurada."
            );

        builder.Services.AddScoped<IAmazonSQS>(_ =>
        {
            return new AmazonSQSClient(
                RegionEndpoint.GetBySystemName(awsRegion)
            );
        });

        builder.Services.AddScoped<ISqsService, SqsService>();

        builder.Services.AddScoped<IDbConnection>(_ =>
        {
            var dbUrl = Environment.GetEnvironmentVariable("DB_URL");
            var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (string.IsNullOrWhiteSpace(dbUrl))
                throw new InvalidOperationException("Variável de ambiente DB_URL não configurada.");

            if (string.IsNullOrWhiteSpace(dbUsername))
                throw new InvalidOperationException("Variável de ambiente DB_USERNAME não configurada.");

            if (string.IsNullOrWhiteSpace(dbPassword))
                throw new InvalidOperationException("Variável de ambiente DB_PASSWORD não configurada.");

            var connectionString = MontarConnectionStringPostgres(dbUrl, dbUsername, dbPassword);

            return new NpgsqlConnection(connectionString);
        });

        builder.Services.AddScoped<IPrecoRepository, PrecoRepository>();
        builder.Services.AddScoped<ISqsService, SqsService>();

        return builder;
    }
}