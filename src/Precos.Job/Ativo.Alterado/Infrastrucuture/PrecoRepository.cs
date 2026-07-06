using Amazon.Runtime.Internal.Util;
using Ativo.Alterado.Domain;
using Ativo.Alterado.Infrastrucuture;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;

public class PrecoRepository : IPrecoRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger<PrecoRepository> _logger;
    public PrecoRepository(IDbConnection connection, ILogger<PrecoRepository> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task InsertPrecoAsync(RegistroPrecificacao registro)
    {
        _logger.LogInformation("Inserindo registro de precificação para o ativo {Codigo} com valor base {ValorBase}.", registro.Codigo, registro.ValorBase);
        
        const string sql = @"
            INSERT INTO precos.precificacao
                (codigo_ativo, preco, data_hora_atualizacao, atualizado)
            VALUES
                (@Codigo, @ValorBase, @DataHoraAtualizacao, @Atualizado);
        ";

        var execution = await _connection.ExecuteAsync(sql, new
        {
            registro.Codigo,
            registro.ValorBase,
            DataHoraAtualizacao = ObterDataHoraSaoPaulo(),
            Atualizado = registro.AptaNegociacao
        });

        if(execution != 1)
        {
            _logger.LogError("Erro ao inserir registro de precificação para o ativo {Codigo}. Linhas afetadas: {LinhasAfetadas}", registro.Codigo, execution);
            throw new Exception($"Erro ao inserir registro de precificação para o ativo {registro.Codigo}. Linhas afetadas: {execution}");
        }

    }

    private static DateTime ObterDataHoraSaoPaulo()
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
    }
}