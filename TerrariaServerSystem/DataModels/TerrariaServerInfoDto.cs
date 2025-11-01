namespace TerrariaServerSystem.DataModels;

public class TerrariaServerInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "未命名";
    public string WorldName { get; set; } = "";
    public int Port { get; set; } = 7777;
    public string Passwd { get; set; } = "";
    public static TerrariaServerInfoDto Create(int key, TerrariaServerInfo info)
    {
        return new TerrariaServerInfoDto()
        {
            Id = key,
            Name = info.Name,
            WorldName = info.WorldName,
            Port = info.Port,
            Passwd = info.Passwd,
        };
    }

    public TerrariaServerInfo Prototype()
    {
        return new TerrariaServerInfo()
        {
            Name = Name,
            WorldName = WorldName,
            Port = Port,
            Passwd = Passwd,
        };
    }

    public override string ToString()
    {
        return $$"""
            Key: {{Id}}
            服务器名称: {{Name}}
            世界名称: {{WorldName}}
            所用端口: {{Port}}
            服务器密码: {{Passwd}}
            """;
    }
}
