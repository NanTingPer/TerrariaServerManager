using System.Diagnostics;
using System.Text;
using TerrariaServerSystem.Interface;

namespace TerrariaServerSystem;

public class Server/* : IServer*/
{
    public long Id { get; set; }
    public string Name { get; set; } = "未命名";

    public ServerConfigOptions ServerOptions { get; init; }

    /// <summary>
    /// 读取进程的标准输出时触发
    /// </summary>
    public event Action<Server, string>? ReadOutputEvent;
    /// <summary>
    /// 此服务器进程
    /// </summary>
    public Process Process { get; init; }

    /// <summary>
    /// 此服务器进程ID
    /// </summary>
    public int ProcessId { get; init; }

    public readonly ProcessStartInfo processStartInfo = new ProcessStartInfo()
    {
        UseShellExecute = false, //不使用命令行执行
        CreateNoWindow = true,
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        StandardErrorEncoding = Encoding.UTF8,
        StandardInputEncoding = Encoding.Unicode, //需要Unicode 前面的问题 就是他！
        StandardOutputEncoding = Encoding.UTF8
    };

    /// <summary>
    /// 创建并Start
    /// </summary>
    public Server(string serverFileName, ServerConfigOptions config)
    {
        processStartInfo.FileName = serverFileName;
        processStartInfo.WorkingDirectory = Path.GetDirectoryName(serverFileName)!;
        config.World = Path.Combine(processStartInfo.WorkingDirectory, "Worlds");

        ServerOptions = config;
        var configs = config.Configs;
        processStartInfo.Arguments = string.Join(' ', configs);
        Process = Process.Start(processStartInfo)!;
        ProcessId = Process.Id;

        Process.OutputDataReceived += ReadLine;
        Process.BeginOutputReadLine();
    }

    private void ReadLine(object sender, DataReceivedEventArgs e)
    {
        ReadOutputEvent?.Invoke(this, e.Data ?? "");
    }

    #region 采用事件触发读取
    //public Task Run() => Run(CancellationToken.None);

    //public async Task Run(CancellationToken token)
    //{
    //    await Task.Delay(1000, token);
    //
    //    await Task.Run(() => {
    //        try {
    //            while (!token.IsCancellationRequested && Process != null && !Process.HasExited) {
    //                int readChar = Process.StandardOutput.Read();
    //                if (readChar == -1)
    //                    continue;
    //
    //                ReadOutputEvent?.Invoke(this, (char)readChar);
    //            }
    //        } finally {
    //            Stop().Wait(1000);
    //        }
    //        
    //    }, token);
    //}
    #endregion

    public Task Stop()
    {
        try {
            if (Process != null && !Process.HasExited) {
                Process.Kill(true);
                Process.WaitForExit(TimeSpan.FromSeconds(4));
            }
        } finally {
            Process?.Dispose();
        }
        return Task.CompletedTask;
    }
}
