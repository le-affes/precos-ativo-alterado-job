using Ativo.Alterado.Domain;

namespace Ativo.Alterado.Infrastrucuture;

public interface IPrecoRepository
{
    public Task InsertPrecoAsync(RegistroPrecificacao registro);
}