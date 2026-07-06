using static Ativo.Alterado.Infrastrucuture.WebApplicationBuilder.BuildHost;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = GetBuilder(args);

        var host = builder.Build();

        using var scope = host.Services.CreateScope();

        host.Run();
    }
}