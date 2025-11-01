//using TerrariaServerSystemTestRun;

//namespace TerrariaServerAPI;

//[Obsolete]
//public class ExitServer(ServerManager server) : BackgroundService
//{
//    private readonly ServerManager _manager = server;
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested) {
//            await Task.Delay(10000, stoppingToken);
//        }
//    }

//    public override async Task StopAsync(CancellationToken cancellationToken)
//    {
//        List<Task> tasks = [];
//        await foreach (var item in _manager.StopAll()) {
//            tasks.Add(item);
//        }
//        ;

//        Task.WaitAll(tasks.ToArray(), cancellationToken);
//    }
//}
