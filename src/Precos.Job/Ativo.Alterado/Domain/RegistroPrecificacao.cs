namespace Ativo.Alterado.Domain;

public class RegistroPrecificacao
{
    public required string Codigo { get; set; }
    public required string Nome { get; set; }
    public required decimal ValorBase { get; set; }
    public required bool AptaNegociacao { get; set; }
}