using Common;
using Login;
using TEngine;

namespace GameLogic
{
    [FGUIConfigAttribute(
        PackageId = LoginPackage.packageId,
        DependentPackageIds = new []{CommonPackage.packageId}
    )]
    public class LoginMainWindow : BindedFGUIBase<UI_LoginMainWindow>
    {
        protected override void InitListeners()
        {
            UI.LoginBtn.onClick.Add(OnLoginBtnClick);
        }

        private void OnLoginBtnClick()
        {
           LoginModule.Instance.ReqLogin(UI.UserNameInput.title.text);
        }
    }
}