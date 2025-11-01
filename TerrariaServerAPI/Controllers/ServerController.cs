using Microsoft.AspNetCore.Mvc;
using TerrariaServerSystem;

namespace TerrariaServerAPI.Controllers;

[ApiController]
[Route("server")]
public class ServerController(ServerManager manager, IConfiguration configuration) : ControllerBase
{
    private ServerManager Manager { get; init; } = manager;

    [HttpPost("add")]
    public IActionResult Add([FromBody] AppendServerRequest request)
    {
        try {
            request.Options.Verify();
        } catch (Exception e) {
            return BadRequest(e.Message);
        }


        var tsPath = configuration.GetValue<string>("tsPath");
        if(tsPath == null || tsPath == string.Empty) {
            return BadRequest("未设置tsPath");
        }

        #region 文件转移 一个服务器一个文件夹
        var appPath = Path.GetDirectoryName(typeof(ServerController).Assembly.Location);
        var path = Path.Combine(appPath!, "servers", request.Name);
        if(!CopyFile(tsPath, path)) {
            return BadRequest("文件复制失败");
        }
        #endregion

        var tsexePath = Path.Combine(path, "TShock.Installer.exe");
        var server = new Server(tsexePath, request.Options);
        var id = Manager.Append(server, request.Name);
        server.ReadOutputEvent += Server_ReadOutputEvent;
        return Ok(id);
    }

    private void Server_ReadOutputEvent(Server arg1, string arg2)
    {
        Console.WriteLine(arg2);
    }

    [HttpPost("delete")]
    public IActionResult Delete([FromBody] long id)
    {
        if (Manager.Stop(id)) {
            return Ok();
        } else {
            return BadRequest("删除失败");
        }
    }

    [HttpPost("list")]
    public IActionResult GetServers()
    {
        return Ok(Manager.Servers.Select(s => new ViewServer() { Id = s.Id, Name = s.Name, Options = s.ServerOptions }).ToList());
    }

    [HttpPost("log")]
    public IActionResult GetLog([FromBody] long id)
    {
        return Ok(Manager.GetLogs(id));
    }

    [NonAction]
    public static bool CopyFile(string? origDirectory, string? newDirectory)
    {
        //origDirectory = Path.GetDirectoryName(origDirectory);
        //newDirectory = Path.GetDirectoryName(newDirectory);

        if (origDirectory == null || newDirectory == null) {
            return false;
        }

        if (!Directory.Exists(newDirectory)) {
            Directory.CreateDirectory(newDirectory);
        }

        var origFiles = Directory.GetFiles(origDirectory).ToList();

        var stack = new Stack<string>();
        Directory.GetDirectories(origDirectory).ToList().ForEach(stack.Push); //获取全部文件夹并入栈

        while (stack.Count != 0) {                                         //递归获取目录内的全部文件
            var dir = stack.Pop(); //获取文件夹                     //递归获取目录内的全部文件
            Directory.GetFiles(dir).ToList().ForEach(s => {
                origFiles.Add(Path.Combine(dir, s));
            });       
            Directory.GetDirectories(dir).ToList().ForEach(s => {
                stack.Push(Path.Combine(dir, s));
            });    //递归获取目录内的全部文件
        }

        foreach (var orig in origFiles) {
            var computingOrig = orig[(origDirectory.Length + 1)..];  //TODO +1 是为了去掉路径分隔符
            var newFilename = Path.Combine(newDirectory, computingOrig);
            var currentDirectory = Path.GetDirectoryName(newFilename); //创建目标文件夹 
            if (!Directory.Exists(currentDirectory)) {                        //创建目标文件夹
                Directory.CreateDirectory(currentDirectory!);                 //创建目标文件夹
            }

            System.IO.File.Copy(orig, newFilename, true);
        }
        return true;
    }
}

public class AppendServerRequest
{
    public required string Name { get; set; }
    public required ServerConfigOptions Options { get; set; }
}

public class ViewServer
{
    public long Id { get; set; }
    public string Name { get; set; } = "未命名";
    public required ServerConfigOptions Options { get; set; }
}
