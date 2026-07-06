using Amazon.SQS;
using Amazon.SQS.Model;
using Ativo.Alterado.Service.DTOs;
using System.Text.Json;

namespace Ativo.Alterado.Service;


public class SqsService : ISqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _operacaoRegistradaQueueUrl;
    private readonly ILogger<SqsService> _logger;
    public SqsService(IAmazonSQS sqsClient, ILogger<SqsService> logger)
    {
        _sqsClient = sqsClient;

        _operacaoRegistradaQueueUrl = Environment.GetEnvironmentVariable("SQS_ATIVO_ALTERADO_URL")
            ?? throw new InvalidOperationException("Variável de ambiente SQS_OPERACAO_REGISTRADA_URL não configurada.");
        _logger = logger;
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
        try
        {
            _logger.LogInformation("Iniciando poll de mensagens.");
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _operacaoRegistradaQueueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 10
            };
            var response = await _sqsClient.ReceiveMessageAsync(request);

            if (response.Messages == null)
            {
                _logger.LogInformation("Nenhuma mensagem encontrada.");
                return null;
            }
            var mensagem = response.Messages[0];

            var desserialized = JsonSerializer.Deserialize<MessageSqsDTO>(mensagem.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            desserialized!.SetId(mensagem.MessageId, mensagem.ReceiptHandle);

            _logger.LogInformation("Recebida mensagem {MessageId}.", mensagem.MessageId);
            return desserialized;
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao receber mensagem da fila SQS.", ex);
        }
    }

    public async Task CommitMessageAsync(MessageSqsDTO mensagem)
    {
        await _sqsClient.DeleteMessageAsync(_operacaoRegistradaQueueUrl, mensagem.ReceiptHandle);
    }
}