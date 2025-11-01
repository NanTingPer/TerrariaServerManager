using TerrariaServerSystem.DataModels;

namespace TerrariaServerSystem.Interface;

public interface ICreateWorld
{
    event Action<int> ChooseWorldEvent;
    int ChooseWorldCount();
    Task CreateWorld(WorldInfo info);
}
