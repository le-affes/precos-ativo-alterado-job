using Amazon.SQS;
using Amazon.SQS.Model;

namespace Ativo.Alterado.Services;


public class SqsPublisherService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _operacaoRegistradaQueueUrl;

    public SqsPublisherService(IAmazonSQS sqsClient)
    {
        _sqsClient = sqsClient;

        _operacaoRegistradaQueueUrl = Environment.GetEnvironmentVariable("SQS_OPERACAO_REGISTRADA_URL")
            ?? throw new InvalidOperationException("Variável de ambiente SQS_OPERACAO_REGISTRADA_URL não configurada.");
    }

    public async Task<string> EnviarMensagemTesteAsync()
    {
        var agora = DateTimeOffset.Now;

        var mensagem = $"olá imola - {agora:yyyy-MM-dd HH:mm:ss zzz}";

        var request = new SendMessageRequest
        {
            QueueUrl = _operacaoRegistradaQueueUrl,
            MessageBody = mensagem,

            // Obrigatório para fila FIFO.
            MessageGroupId = "operacao-registrada-teste",

            // Necessário se a fila FIFO não estiver com ContentBasedDeduplication ativo.
            MessageDeduplicationId = Guid.NewGuid().ToString()
        };

        var response = await _sqsClient.SendMessageAsync(request);

        return response.MessageId;
    }
}