namespace TerrariaServerSystem.DataModels;

public record class TerrariaServerInfo
{
    public string Name { get; set; } = "未命名";
    public string WorldName { get; set; } = "";
    public int Port { get; set; } = 7777;
    public string Passwd { get; set; } = "";
}
