using Ativo.Alterado.Domain;

namespace Ativo.Alterado.Service.DTOs;

public class MessageSqsDTO
{
    public required string Codigo { get; set; }
    public required string Nome { get; set; }
    public required decimal Valor_Base { get; set; }
    public required bool Apta_Negociacao { get; set; }

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