using System;
using System.Threading.Tasks;
using Google.Protobuf;
using IoGame.Gen;
using IoGame.Sdk;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public sealed partial class NetworkModule : Singleton<NetworkModule>
    {
        public NetworkWebSocket NetworkWebSocket{private set; get; }
        
        protected override void OnInit()
        {
            base.OnInit();
            
            // biz code init
            GameCode.Init();
            //Index.Listen();

            // --------- IoGameSetting ---------
            IoGameSetting.EnableDevMode = true;
            // China or Us
            IoGameSetting.SetLanguage(IoGameLanguage.China);
            // message callback. 回调监听
            IoGameSetting.ListenMessageCallback = new NetworkListenMessageCallback();
            IoGameSetting.GameGameConsole = new NetworkConsole();

            // socket
            SocketInit();
        }
        
        private void SocketInit()
        {
            IoGameSetting.Url = "ws://127.0.0.1:10100/websocket";

            // socket
            NetworkWebSocket = new NetworkWebSocket();
            IoGameSetting.NetChannel = NetworkWebSocket;

            NetworkWebSocket.OnOpen += (_, _) =>
            {
                Log.Info("WebSocket OnOpen new!");
                IdleTimer();
                
                // login
                //var loginVerifyMessage = new LoginVerifyMessage { Jwt = "1234567" };
                //SdkAction.OfLoginVerify(loginVerifyMessage, result =>
                //{
                //    var userMessage = result.GetValue<UserMessage>();
                //    result.Log($"userMessage: {userMessage}");
                //});

                // heartbeat
            };
        }

        public void StartNet()
        {
            IoGameSetting.StartNet();
        }
        
        private async void IdleTimer()
        {
            var heartbeatMessage = new ExternalMessage().ToByteArray();
            var counter = 0;

            while (true)
            {
                await Task.Delay(8000);
                Debug.Log($"-------- unity new ...HeartbeatMessage {counter++}");
                // Send heartbeat to server. 发送心跳给服务器
                IoGameSetting.NetChannel.WriteAndFlush(heartbeatMessage);
            }
        }
    }
}