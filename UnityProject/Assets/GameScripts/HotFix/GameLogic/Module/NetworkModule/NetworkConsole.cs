using IoGame.Sdk;

namespace GameLogic
{
    public class NetworkConsole : IGameConsole
    {
        public void Log(object value)
        {
            TEngine.Log.Info($"网络-> {value}");
        }
    }
}