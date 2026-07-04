using Amazon.SQS;
using Amazon.SQS.Model;
using Ativo.Alterado.Service.DTOs;
using System.Text.Json;

namespace Ativo.Alterado.Service;


public class SqsService : ISqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _operacaoRegistradaQueueUrl;

    public SqsService(IAmazonSQS sqsClient)
    {
        _sqsClient = sqsClient;

        _operacaoRegistradaQueueUrl = Environment.GetEnvironmentVariable("SQS_ATIVO_ALTERADO_URL")
            ?? throw new InvalidOperationException("Variável de ambiente SQS_OPERACAO_REGISTRADA_URL não configurada.");
    }

    public async Task<string> SendMessageAsync()
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

    public async Task<MessageSqsDTO?> ReceiveMessageAsync()
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _operacaoRegistradaQueueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 10
        };
        var response = await _sqsClient.ReceiveMessageAsync(request);
        if (response.Messages.Count == 0)
        {
            return null;
        }
        var mensagem = response.Messages[0];

        await _sqsClient.DeleteMessageAsync(_operacaoRegistradaQueueUrl, mensagem.ReceiptHandle);
        var desserialized = JsonSerializer.Deserialize<MessageSqsDTO>(mensagem.Body);

        return desserialized;
    }
}