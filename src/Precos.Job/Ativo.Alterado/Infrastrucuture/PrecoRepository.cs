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

    private sealed class ResultadoPrecificacao
    {
        public int Desativadas { get; init; }
        public int Inseridas { get; init; }
    }

    public async Task InsertPrecoAsync(RegistroPrecificacao registro)
    {
        _logger.LogInformation(
            "Processando precificação para o ativo {Codigo}. Valor base: {ValorBase}, Apta para negociação: {AptaNegociacao}.",
            registro.Codigo,
            registro.ValorBase,
            registro.AptaNegociacao);

        const string sql = """
                        WITH precificacoes_desativadas AS
                        (
                            UPDATE precos.precificacao
                            SET atualizado = FALSE
                            WHERE codigo_ativo = @Codigo
                              AND atualizado = TRUE
                            RETURNING 1
                        ),
                        precificacao_inserida AS
                        (
                            INSERT INTO precos.precificacao
                            (
                                codigo_ativo,
                                preco,
                                data_hora_atualizacao,
                                atualizado
                            )
                            SELECT
                                @Codigo,
                                @ValorBase,
                                @DataHoraAtualizacao,
                                TRUE
                            WHERE @AptaNegociacao = TRUE
                            RETURNING 1
                        )
                        SELECT
                            (SELECT COUNT(*) FROM precificacoes_desativadas) AS Desativadas,
                            (SELECT COUNT(*) FROM precificacao_inserida) AS Inseridas;
                        """;

        var resultado = await _connection.QuerySingleAsync<ResultadoPrecificacao>(
            sql,
            new
            {
                registro.Codigo,
                registro.ValorBase,
                registro.AptaNegociacao,
                DataHoraAtualizacao = ObterDataHoraSaoPaulo()
            });

        if (registro.AptaNegociacao && resultado.Inseridas != 1)
        {
            _logger.LogError(
                "Erro ao inserir a nova precificação do ativo {Codigo}. Linhas inseridas: {Inseridas}.",
                registro.Codigo,
                resultado.Inseridas);

            throw new InvalidOperationException(
                $"Não foi possível inserir a precificação do ativo {registro.Codigo}.");
        }

        _logger.LogInformation(
            "Precificação processada para o ativo {Codigo}. Desativadas: {Desativadas}; inseridas: {Inseridas}.",
            registro.Codigo,
            resultado.Desativadas,
            resultado.Inseridas);
    }

    private static DateTime ObterDataHoraSaoPaulo()
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
    }
}