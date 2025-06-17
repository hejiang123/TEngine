using Pb.Common;

namespace GameLogic
{
    public sealed partial class UserModule : Singleton<UserModule>
    {
        public UserInfo UserInfo { get; private set; }
        protected override void OnInit()
        {
            base.OnInit();
        }

        public void SetUserInfo(UserInfo userInfo)
        {
            this.UserInfo = userInfo;
        }
        
        
    }
}