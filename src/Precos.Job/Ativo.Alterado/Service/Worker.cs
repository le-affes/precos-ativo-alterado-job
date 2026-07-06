using Ativo.Alterado.Infrastrucuture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ativo.Alterado.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPrecoRepository _precoRepository;
    private readonly ISqsService _sqsService;
    private readonly IServiceScopeFactory _scopeFactory;
    public Worker(ILogger<Worker> logger, IPrecoRepository precoRepository, ISqsService sqsService, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _precoRepository = precoRepository;
        _sqsService = sqsService;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mensagem = await _sqsService.ReceiveMessageAsync();

                if (mensagem is null)
                {
                    continue;
                }

                await _precoRepository.InsertPrecoAsync(mensagem.MapToDomain());

                await _sqsService.CommitMessageAsync(mensagem);

                _logger.LogInformation("Mensagem processada com sucesso: {@Mensagem}", mensagem.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consumir mensagem da fila SQS.");
            }
        }

        _logger.LogInformation("Worker finalizado.");
    }
}