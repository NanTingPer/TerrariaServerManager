using System.Diagnostics;
using System.Text;
using TerrariaServerSystem.DataModels;
using TerrariaServerSystem.Exceptions;
using TerrariaServerSystem.Interface;

namespace TerrariaServerSystem.Obsoletes;

[Obsolete(message: "已经过时")]
public class TerrariaServer : ICreateWorld, IServer
{
    private enum WriteStatus
    {
        ChoiceWorld
    }

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    private TerrariaServer() 
    {
        //startInfo.ArgumentList.Add();
        ServerProcess = Process.Start(startInfo)!;
        ProcessId = ServerProcess.Id;
        StartEvent?.Invoke(ProcessId);
        ServerProcess.StandardInput.AutoFlush = true;
    }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。

    /// <summary>
    /// 传入ProcessId
    /// </summary>
    public event Action<int>? StartEvent;
    public event Action<TerrariaServer>? StopEvent;

    private event Action<int>? chooseWorldEvent;
    event Action<int>? ICreateWorld.ChooseWorldEvent
    {
        add
        {
            chooseWorldEvent += value;
        }
        remove
        {
            chooseWorldEvent -= value;
        }
    }
    #region Private Field
    private int chooseWorldCount = 0;
    private bool isRun = false;
    //private WriteStatus status = WriteStatus.ChoiceWorld;
    private readonly static ProcessStartInfo startInfo = new ProcessStartInfo()
    {
        UseShellExecute = false, //不使用命令行执行
        CreateNoWindow = true,
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        StandardErrorEncoding = Encoding.UTF8,
        StandardInputEncoding = Encoding.Unicode, //需要Unicode 前面的问题 就是他！
        StandardOutputEncoding = Encoding.UTF8,
        FileName = Environment.GetEnvironmentVariable("TSPATH") ?? throw new NotVariable("没找到环境变量TSPATH，值为TerrariaServer的完整路径，或者Ts的完整路径") //"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Terraria\\TerrariaServer.exe"
    };

    public Process? ServerProcess { get; private set; }
    private List<string> logOutput = [];
    #endregion

    public TerrariaServerInfo Info { get; init; }
    public TerrariaServer(TerrariaServerInfo info) : this()
    {
        Info = info;
    }

    public int ProcessId { get; private set; }

    private CancellationTokenSource taskToken = new CancellationTokenSource();
    public async Task Run()
    {
        if (isRun)
            return;
        isRun = true;
        await RunTerrariaCLI(WriteLine);
        return ;
    }

    public async Task ReStart()
    {
        await Stop();
        _ = Run();
    }

    private async Task WriteLine(int _)
    {
        try {
            await ChoiceWorld();
        } catch {
            await Stop();
            throw;
        }
    }

    /// <exception cref="WorldListIsNullException"></exception>
    /// <exception cref="NotWorldException"></exception>
    private async Task ChoiceWorld()
    {
        //计算是否有世界，没有就返回空
        var startIndex = logOutput.FindIndex(f => f.StartsWith("Terraria Server"));
        var lastIndex = logOutput.FindIndex(f => f.StartsWith("d <number>"));
        if (startIndex + 2 == lastIndex) {
            throw new WorldListIsNullException();
        }

        //计算最后一个世界的索引
        var oneWorld = logOutput[lastIndex - 2];
        (int worldCount, string _) s = new WorldText(oneWorld).IndexName 
            ?? throw new WorldListIsNullException();

        var worldCount = s.worldCount;

        Dictionary<int, string> worldList = [];
        //根据最后一个世界的索引 遍历获取世界列表
        for (int i = 0; i < worldCount; i++) {
            var worldText = logOutput[lastIndex - 2 - i];
            (int worldIndex, string worldName)? @in = new WorldText(worldText).IndexName;
            if (@in == null) continue;
            worldList.Add(@in.Value.worldIndex, @in.Value.worldName);
        }

        if (!worldList.ContainsValue(Info.WorldName)) 
            throw new NotWorldException($"未找到给定世界: {Info.WorldName}");

        //选择世界
        var index = worldList.FirstOrDefault(kv => kv.Value.Equals(Info.WorldName)).Key;
        await Task.Delay(1000);
        //Thread.Sleep(1000);
        ServerProcess!.StandardInput.WriteLine(index);

        await Task.Delay(500);
        //Thread.Sleep(500);
        //设置人数
        ServerProcess!.StandardInput.WriteLine(); //默认16

        await Task.Delay(500);
        //Thread.Sleep(500);
        //设置端口
        ServerProcess.StandardInput.WriteLine(Info.Port);

        await Task.Delay(500);
        //Thread.Sleep(500);
        //自动端口转发
        ServerProcess.StandardInput.WriteLine();

        await Task.Delay(500);
        //Thread.Sleep(500);
        //服务器密码
        ServerProcess.StandardInput.WriteLine(Info.Passwd);
    }

    async Task ICreateWorld.CreateWorld(WorldInfo info)
    {
        ServerProcess!.StandardInput.AutoFlush = true;
        await RunTerrariaCLI(async (coun) => {
            if(coun == 1) {
                try {
                    await Task.Delay(1000);
                    ServerProcess!.StandardInput.WriteLine("n" + ServerProcess.StandardInput.NewLine);
                    await Task.Delay(1000);
                    ServerProcess!.StandardInput.WriteLine(info.WorldSize);
                    await Task.Delay(1000);
                    ServerProcess!.StandardInput.WriteLine(info.WorldDifficulty);
                    await Task.Delay(1000);
                    ServerProcess!.StandardInput.WriteLine(info.WorldEvil);
                    await Task.Delay(1000);
                    ServerProcess!.StandardInput.WriteLine(info.WroldName);
                    await Task.Delay(1000);
                    ServerProcess!.StandardInput.WriteLine(info.WroldSeed);
                } catch {
                    throw;
                }
            } else if(coun == 2) {
                await Stop();
                Console.WriteLine("退出");
            }
        });
    }

    public async Task Stop()
    {
        try {
            StopEvent?.Invoke(this);
            ServerProcess?.StandardInput.WriteLine("exit");
            await Task.Delay(5000);
            //ServerProcess?.WaitForExit(1000);
            //if(!ServerProcess?.HasExited ?? false) {
            ServerProcess?.Kill();
            //}
        } finally {
            ServerProcess = null;
            ServerProcess?.Dispose();
            taskToken.Cancel();
        }
    }

    private async Task RunTerrariaCLI(Func<int, Task> func)
    {
        StringBuilder line = new StringBuilder();
        string lineText = "";
        while (ServerProcess != null && !ServerProcess.HasExited) {
            char[] @char = new char[1];
            ServerProcess.StandardOutput.Read(@char, 0, 1);
            if (@char[0] != Environment.NewLine[0]) { //不是换行符
                line.Append(@char);
                lineText = line.ToString();
                if (lineText.StartsWith('\n') || lineText.StartsWith('\r')) {
                    lineText = lineText[1..];
                }
            } else { //是换行符
                if (!string.IsNullOrWhiteSpace(lineText)) {
                    logOutput.Add(lineText);
                }
                line.Clear();
            }

            Console.Write(@char);
            if (lineText.Equals("Choose World: ")) {
                chooseWorldCount++;
                await func.Invoke(chooseWorldCount);
                chooseWorldEvent?.Invoke(chooseWorldCount);
            }
        }
    }

    int ICreateWorld.ChooseWorldCount()
    {
        return chooseWorldCount;
    }

    public static ICreateWorld CreateWorld()
    {
        return new TerrariaServer();
    }
}
