using FairyGUI;

namespace GameLogic
{
    /// <summary>
    /// 安全区域界面适配器
    /// </summary>
    public class SafeAreaAdaptor : UIAdaptorBase
    {
        public override void Initialize(GComponent contentPane, GComponent uiRoot)
        {
            base.Initialize(contentPane, uiRoot);
            UpdateSafeArea();
        }

        public void UpdateSafeArea()
        {
            var safeArea = FGUIModule.Instance.UISafeArea;
            _contentPane.SetSize(safeArea.width, safeArea.height);
            _contentPane.SetXY(safeArea.x, _uiRoot.height - safeArea.y - safeArea.height);
        }
    }
}