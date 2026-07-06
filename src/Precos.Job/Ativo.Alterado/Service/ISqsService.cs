using Ativo.Alterado.Service.DTOs;

namespace Ativo.Alterado.Service;

public interface ISqsService
{
    public Task<MessageSqsDTO?> ReceiveMessageAsync();
    public Task CommitMessageAsync(MessageSqsDTO mensagem);
}