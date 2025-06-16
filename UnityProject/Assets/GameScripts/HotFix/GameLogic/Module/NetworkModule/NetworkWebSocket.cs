using System;
using IoGame.Sdk;
using TEngine;
using UnityWebSocket;

namespace GameLogic
{
    public class NetworkWebSocket : SimpleNetChannel
    {
        public string Url;

        private WebSocket _socket;

        public event EventHandler<OpenEventArgs> OnOpen = (_, _) => { Log.Info("WebSocket OnOpen"); };

        public override void Prepare()
        {
            Url ??= IoGameSetting.Url;
            _socket = new WebSocket(Url);
            // 注册回调
            _socket.OnOpen += OnOpen;
            _socket.OnClose += (_, e) => { Log.Info($"Connection closed! {e}"); };
            _socket.OnError += (_, e) => { Log.Error($"e {e}"); };
            _socket.OnMessage += (m, e) =>
            {
                var packet = e.RawData;
                AcceptMessage(ExternalMessage.Parser.ParseFrom(packet));
            };

            // 连接
            _socket.ConnectAsync();
        }

        public override void WriteAndFlush(byte[] bytes)
        {
            _socket.SendAsync(bytes);
        }
    }
}