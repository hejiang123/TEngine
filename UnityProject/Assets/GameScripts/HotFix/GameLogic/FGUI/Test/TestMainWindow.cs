using Common;
using TEngine;
using Test;

namespace GameLogic
{
    [FGUIConfigAttribute(
        PackageId = TestPackage.packageId,
        DependentPackageIds = new []{CommonPackage.packageId}
        )]
    public class TestMainWindow : BindedFGUIBase<UI_TestMainWindow>
    {
        private FullScreenAdaptor _adaptor;
        protected override void InitListeners()
        {
            UI.Btn1.onClick.Add((() =>
            {
                Log.Error($"打开其他窗口");
            }));
        }

        public override void OnCreate()
        {
            base.OnCreate();
            
            _adaptor = new FullScreenAdaptor();
            _adaptor.Initialize(UI, Parent);
        }
    }
}