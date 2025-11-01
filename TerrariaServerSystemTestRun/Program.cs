using TerrariaServerSystem;

namespace TerrariaServerSystemTestRun;

public class Program
{

    public async Task Run()
    {
        var rootDirectory = Environment.CurrentDirectory;
        var worldDirectory = Path.Combine(rootDirectory, "Worlds");

        await Task.CompletedTask;
    }

    static async Task Main(string[] args)
    {
        var co = new ServerConfigOptions()
        {
            AutoCreate = "1",
            WorldName = "testworld",
        };

        co.World = $@"C:\TShock-5.2.4\Worlds\{co.WorldName}.wld";

        var server = new Server(@"C:\TShock-5.2.4\TShock.Installer.exe", co);
        server.ReadOutputEvent += CWLoging;
        var serverManagser = new ServerManager();
        serverManagser.Append(server);
        await server.Run();

        while (true) {

        }
    }

    private static void CWLoging(Server server, char obj)
    {
        Console.Write(obj);
    }
}