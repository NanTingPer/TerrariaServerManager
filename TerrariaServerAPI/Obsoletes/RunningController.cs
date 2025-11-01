//using Microsoft.AspNetCore.Mvc;
//using TerrariaServerSystemTestRun;

//namespace TerrariaServerAPI.Controllers;

//[ApiController]
//public class RunningController(ServerManager manager) : Controller
//{
//    [HttpPost("create")]
//    public async Task<IActionResult> Create([FromBody] TerrariaServerInfo info)
//    {
//        await manager.AddServer(info);
//        return Ok(manager.Servers);
//    }

//    [HttpPost("stop")]
//    public async Task<IActionResult> Stop([FromBody] int key)
//    {
//        await manager.StopServer(key);
//        return Ok(manager.Servers);
//    }

//    [HttpPost("restart")]
//    public async Task<IActionResult> Restart ([FromBody] int key)
//    {
//        await manager.ReStart(key);
//        return Ok();
//    }

//    [HttpPost("update")]
//    public async Task<IActionResult> Update([FromBody] TerrariaServerInfoDto dto)
//    {
//        await manager.UpdateInfo(dto.Id, dto.Prototype());
//        return Ok();
//    }
//}
