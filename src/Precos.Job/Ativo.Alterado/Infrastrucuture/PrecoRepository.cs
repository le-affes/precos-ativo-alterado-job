using Ativo.Alterado.Domain;
using Ativo.Alterado.Infrastrucuture;
using Dapper;
using System.Data;

public class PrecoRepository : IPrecoRepository
{
    private readonly IDbConnection _connection;

    public PrecoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task InserirPrecificacaoAsync(RegistroPrecificacao registro)
    {
        const string sql = @"
            INSERT INTO precos.precificacao
                (codigo_ativo, preco, data_hora_atualizacao, atualizado)
            VALUES
                (@Codigo, @ValorBase, @DataHoraAtualizacao, @Atualizado);
        ";

        await _connection.ExecuteAsync(sql, new
        {
            registro.Codigo,
            registro.ValorBase,
            DataHoraAtualizacao = ObterDataHoraSaoPaulo(),
            Atualizado = registro.AptaNegociacao
        });
    }

    private static DateTime ObterDataHoraSaoPaulo()
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
    }
}