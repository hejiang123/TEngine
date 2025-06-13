using FairyGUI;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 全屏界面适配器
    /// </summary>
    public class FullScreenAdaptor : UIAdaptorBase
    {
        private GObject _safeAreaObject;
    
        public override void Initialize(GComponent contentPane, GComponent uiRoot)
        {
            base.Initialize(contentPane, uiRoot);
        
            // 设置全屏尺寸
            _contentPane.SetSize(_uiRoot.width, _uiRoot.height);
            _contentPane.SetXY(0, 0);
            _contentPane.AddRelation(_uiRoot, RelationType.Size);
        
            // 查找安全区域节点
            _safeAreaObject = _contentPane.GetChild("safeArea");
            if (_safeAreaObject != null)
            {
                _safeAreaObject.relations.ClearAll();
                UpdateSafeArea();
            }
        }

        public override void Dispose()
        {
            _contentPane.RemoveRelation(_uiRoot, RelationType.Size);
            base.Dispose();
        }

        public void UpdateSafeArea()
        {
            if (_safeAreaObject == null) return;
        
            var safeArea = FGUIModule.Instance.UISafeArea;
            _safeAreaObject.SetSize(safeArea.width, safeArea.height);
            _safeAreaObject.SetXY(safeArea.x, _uiRoot.height - safeArea.y - safeArea.height);
        }
    }
}