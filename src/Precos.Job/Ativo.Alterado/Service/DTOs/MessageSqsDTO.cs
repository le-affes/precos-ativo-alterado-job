using Ativo.Alterado.Domain;

namespace Ativo.Alterado.Service.DTOs;

public record MessageSqsDTO
{
    public string? MessageId { get; private set; }
    public string? ReceiptHandle { get; private set; }
    public required string Codigo { get; set; }
    public required string Nome { get; set; }
    public required decimal Valor_Base { get; set; }
    public required bool Apta_Negociacao { get; set; }

    public void SetId(string messageId, string messageHandle)
    {
        MessageId = messageId;
        ReceiptHandle = messageHandle;
    }

    public RegistroPrecificacao MapToDomain()
    {
        return new RegistroPrecificacao
        {
            Codigo = Codigo,
            Nome = Nome,
            ValorBase = Valor_Base,
            AptaNegociacao = Apta_Negociacao
        };
    }
}