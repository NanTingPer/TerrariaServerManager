using TerrariaServerSystem.DataModels;

namespace TerrariaServerSystem.Obsoletes;

[Obsolete(message: "此服务管理器已过时")]
public class ServerManager
{
    private readonly Dictionary<int, TerrariaServer> _servers = [];
    private readonly Dictionary<int, TerrariaServerInfo> _infos = [];
    public List<TerrariaServerInfoDto> Servers => [.. _infos.Select(f => TerrariaServerInfoDto.Create(f.Key, f.Value))];
    private readonly SemaphoreSlim _editSlim = new SemaphoreSlim(1, 1);

    private async void StopEventMethod(TerrariaServer t)
    {
        await IfThrowExple(async () => {
            int key = -1;
            foreach (var item in _servers) {
                if (item.Value.Equals(t)) {
                    key = item.Key;
                    break;
                }
            }
            _servers.Remove(key);
            _infos.Remove(key);
            await Task.CompletedTask;
        });
        
    }

    /// <summary>
    /// 运行此info的服务器，并返回唯一标识
    /// </summary>
    public async Task<int> AddServer(TerrariaServerInfo info)
    {
        var newKey = -1;
        await IfThrowExple(async () => {
            TerrariaServer server = CreateTerrariaServer(info);
            if (_servers.Count != 0) {
                newKey = _servers.Keys.Max() + 1;
            } else {
                newKey = 0;
            }
            _ = WhenThrowTask(newKey, server.Run());
            _servers.Add(newKey, server);
            _infos.Add(newKey, info);
            await Task.CompletedTask;
        }, async () => {
            await StopServer(newKey);
        });
        return newKey;
    }

    /// <summary>
    /// 如果执行<paramref name="action"/>过程中发生错误，那么释放信号量并执行<paramref name="catchAction"/>
    /// </summary>
    private async Task IfThrowExple(Func<Task> func, Action? catchAction = null)
    {
        bool isExcep = false;
        Exception? exception = null;
        await _editSlim.WaitAsync();
        try {
            await func.Invoke();
        } catch (Exception excep) {
            isExcep = true;
            exception = excep;
            _editSlim.Release(); //? 避免catchaction执行过长 提前释放，如果catchaction有获取锁也可能正常获取
            try {
                catchAction?.Invoke();
            } catch { }
        }
        //_editSlim.Release();
        if (isExcep) {
            throw exception!;
        } else {                    //?     正常执行完成后的释放
            _editSlim.Release();    //?     正常执行完成后的释放
        }                           //?     正常执行完成后的释放
    }


    private TerrariaServer CreateTerrariaServer(TerrariaServerInfo info)
    {
        var server = new TerrariaServer(info);
        server.StopEvent += StopEventMethod;
        return server;
    }

    private async Task Cover(int key, TerrariaServerInfo info)
    {
        await IfThrowExple(async () => {
            TerrariaServer server = CreateTerrariaServer(info);
            _ = WhenThrowTask(key, server.Run());
            _servers[key] = server;
            _infos[key] = info;
            await Task.CompletedTask;
        }, async () => {
            await StopServer(key);
        });
    }

    private async Task WhenThrowTask(int key, Task task)
    {
        try {
            await task;
        } catch {
            await StopServer(key);
        }
    }

    /// <summary>
    /// 停止指定标识的服务器
    /// </summary>
    public async Task StopServer(int identity)
    {
        await IfThrowExple(async () => {
            if (_servers.TryGetValue(identity, out var server)) {
                await server.Stop();
                _servers.Remove(identity);
            }
        }, () => {
            _servers.Remove(identity);
        });
    }

    public async IAsyncEnumerable<Task> StopAll()
    {
        await Task.CompletedTask;
        foreach (var item in _servers.Values) {
            yield return Task.Run(() => item.Stop());
        }
        _servers.Clear();
    }

    /// <summary>
    /// 更新并启动
    /// </summary>
    public async Task UpdateInfo(int identity, TerrariaServerInfo newInfo)
    {
        await IfThrowExple(async () => {
            if (_servers.TryGetValue(identity, out var server)) {
                await server.Stop();
                _servers.Remove(identity);
                _infos.Remove(identity);
            }
        });

        await Cover(identity, newInfo);
    }

    /// <summary>
    /// 重启此标识代表的服务器
    /// </summary>
    public async Task ReStart(int identity)
    {
        if(_servers.TryGetValue(identity, out var server)) {
            await server.ReStart();
        }
    }
}