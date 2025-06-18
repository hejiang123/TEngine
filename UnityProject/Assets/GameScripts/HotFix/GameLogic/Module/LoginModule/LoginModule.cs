using IoGame.Gen;
using IoGame.Sdk;
using Pb.Common;
using TEngine;
using UnityWebSocket;

namespace GameLogic
{
    public sealed partial class LoginModule : Singleton<LoginModule>
    {
        private string _loginUserName;
        protected override void OnInit()
        {
            base.OnInit();
        }

        public void ReqLogin(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Log.Error($"账号不能为空");
                return;
            }
            
            _loginUserName = username;
            NetworkModule.Instance.NetworkWebSocket.OnOpen += OnOpen;
            NetworkModule.Instance.StartNet();
        }

        private async void OnOpen(object sender, OpenEventArgs args)
        {
            NetworkModule.Instance.NetworkWebSocket.OnOpen -= OnOpen;
            LoginVerify loginVerify = new LoginVerify();
            loginVerify.UserName = _loginUserName;
            ResponseResult responseResult = await LoginAction.OfAwaitLoginVerify(loginVerify);
            responseResult.Check<UserInfo>((info =>
            {
                UserInfo userInfo = responseResult.GetValue<UserInfo>();
                UserModule.Instance.SetUserInfo(userInfo);
                Log.Error($"登陆成功 {userInfo.NickName}"); 
            }));
        }
    }
}