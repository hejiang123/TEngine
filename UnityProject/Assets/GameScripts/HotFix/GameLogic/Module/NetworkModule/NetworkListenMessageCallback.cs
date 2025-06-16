using IoGame.Sdk;
using TEngine;

namespace GameLogic
{
    public class NetworkListenMessageCallback : SimpleListenMessageCallback
    {
        public override void OnListenCallback(ExternalMessage message, ListenCommand listenCommand)
        {
            base.OnListenCallback(message, listenCommand);
            Log.Info($"OnListenCallback-> {message}");
        }

        public override void OnIdleCallback(ExternalMessage message)
        {
            base.OnIdleCallback(message);
            Log.Info($"OnIdleCallback-> {message}");
        }

        public override void OnErrorCallback(ExternalMessage message, RequestCommand requestCommand)
        {
            base.OnErrorCallback(message, requestCommand);
            Log.Info($"OnErrorCallback-> {message}");
        }

        public override void OnOtherCallback(ExternalMessage message)
        {
            base.OnOtherCallback(message);
            Log.Info($"OnOtherCallback-> {message}");
        }

        public override void OnRequestCallback(ExternalMessage message, RequestCommand requestCommand)
        {
            base.OnRequestCallback(message, requestCommand);
            Log.Info($"OnRequestCallback-> {message}");
        }
    }
}