namespace TerrariaServerSystem.DataModels;

public class WorldInfo
{
    /// <summary>
    /// 世界大小
    /// </summary>
    public int WorldSize { get; set; } = 1;
    /// <summary>
    /// 世界难度
    /// <para>1.普通 2.专家 3.大师 4.旅途</para>
    /// </summary>
    public int WorldDifficulty { get; set; } = 3; //1.普通 2.专家 3.大师 4.旅途
    /// <summary>
    /// 世界邪恶
    /// <para>1.随机 2.腐化 3.猩红</para>
    /// </summary>
    public int WorldEvil { get; set; } = 1; //1.随机 2.腐化 3.猩红
    /// <summary>
    /// 世界名称
    /// </summary>
    public string WroldName { get; set; } = string.Empty;
    /// <summary>
    /// 种子
    /// </summary>
    public string WroldSeed { get; set; } = string.Empty; //种子
}
