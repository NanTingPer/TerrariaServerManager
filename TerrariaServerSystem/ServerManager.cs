using System.Runtime.Loader;
using System.Text;

namespace TerrariaServerSystem;

public class ServerManager
{
    private readonly static CancellationTokenSource s_sourceToken = new();
    private readonly Dictionary<long, StringBuilder> _serverLogs = [];
    /// <summary>
    /// 对全部进程进行退出
    /// </summary>
    static ServerManager()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => ExitStop();
        Console.CancelKeyPress += (_, _) => ExitStop(); 
        AssemblyLoadContext.Default.Unloading += (_) => {
            s_sourceToken.Cancel(true);
            foreach (var item in s_serverManagers) {
                foreach (var item1 in item._servers.Values) {
                    item1.Stop().Wait(3000);
                }
            }
        };
    }

    private static void ExitStop()
    {
        s_sourceToken.Cancel(true);
        foreach (var item in s_serverManagers) {
            foreach (var item1 in item._servers.Values) {
                item1.Stop().Wait(3000);
            }
        }
        Environment.Exit(0);
    }

    private readonly static List<ServerManager> s_serverManagers = [];
    private readonly static object s_lock = new();
    /// <summary>
    /// 进程全部的ServerManager实例
    /// </summary>
    //private static IEnumerable<ServerManager> ServerManagers { get => s_serverManagers.AsEnumerable(); }

    private readonly object _appendlock = new();
    private long _serverCount = 0L; //此ServerManager管理了多少服务器实例，用于生成唯一ID
    private readonly Dictionary<long, Server> _servers = [];

    /// <summary>
    /// 获取此服务器管理器的所管理的全部服务器实例
    /// </summary>
    public IEnumerable<Server> Servers { get => _servers.Select(f => f.Value); }

    public ServerManager()
    {
        lock (s_lock) {
            try {
                s_serverManagers.Add(this);
            } finally { }
        }
    }

    /// <summary>
    /// 添加并运行Run, 这样会使用管理器的 <see cref="CancellationToken"/>
    /// </summary>
    //public long AppendAndRun(Server server, string name)
    //{
    //    //_ = server.Run(s_sourceToken.Token);
    //    return Append(server, name);
    //}

    /// <summary>
    /// 添加一个服务器实例到此管理器中，并返回其唯一ID
    /// </summary>
    public long Append(Server server, string name)
    {
        var count = 0L;
        lock (_appendlock) {
            _serverCount++;
            count = _serverCount;
        }

        var managedServer = server;
        managedServer.Id = count;
        managedServer.Name = name;
        _servers.Add(count, managedServer);
        _serverLogs[count] = new StringBuilder();
        managedServer.ReadOutputEvent += AppendLogs;
        return count;
    }

    private void AppendLogs(Server arg1, string arg2)
    {
       if (_serverLogs.TryGetValue(arg1.Id, out var sb)) {
            sb.Append(arg2);
       }
    }

    /// <summary>
    /// 添加一个服务器实例到此管理器中，并返回其唯一ID
    /// </summary>
    public long Append(Server server) => Append(server, "未命名服务器");

    /// <summary>
    /// 删除一个服务器实例
    /// </summary>
    public bool Remove(long id)
    {
        if(_servers.TryGetValue(id, out var server)) {
            server.Stop().Wait(1000);
        }

        bool isRemove = _servers.Remove(id);
        return isRemove;
    }

    /// <summary>
    /// 停止一个服务器实例(实际调用<see cref="Remove(long)"/>)
    /// </summary>
    public bool Stop(long id) => Remove(id);

    /// <summary>
    /// 停止指定名称的服务器实例(实际调用<see cref="Remove(int)"/>)
    /// </summary>
    public bool Remove(string serverName)
    {
        var tarKeyValue = _servers.FirstOrDefault(f => f.Value.Name == serverName);
        var tarServer = tarKeyValue.Value;
        if (tarServer != null) {
            return Remove(tarKeyValue.Key);
        }
        return false;
    }

    /// <summary>
    /// 停止指定名称的服务器实例(实际调用<see cref="Remove(string)"/>)
    /// </summary>
    public bool Stop(string serverName) => Remove(serverName);

    /// <summary>
    /// 获取服务器的日志
    /// </summary>
    /// <returns> 存在则返回日志, 否则返回<see cref="string.Empty"/> </returns>
    public string GetLogs(long id)
    {
        if(_serverLogs.TryGetValue(id, out var value)) {
            return value.ToString();
        }
        return string.Empty;
    }
}
