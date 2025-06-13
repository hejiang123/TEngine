using FairyGUI;

namespace GameLogic
{
    /// <summary>
    /// 固定尺寸界面适配器
    /// </summary>
    public class ConstantSizeAdaptor : UIAdaptorBase
    {
        private readonly bool _horizontalCenter;
        private readonly bool _verticalCenter;

        public ConstantSizeAdaptor(bool horizontalCenter = true, bool verticalCenter = true)
        {
            _horizontalCenter = horizontalCenter;
            _verticalCenter = verticalCenter;
        }

        public override void Initialize(GComponent contentPane, GComponent uiRoot)
        {
            base.Initialize(contentPane, uiRoot);
        
            // 初始位置设置
            UpdatePosition();
        
            // 添加关联
            if (_horizontalCenter)
            {
                contentPane.AddRelation(uiRoot, RelationType.Center_Center);
            }
        
            if (_verticalCenter)
            {
                contentPane.AddRelation(uiRoot, RelationType.Middle_Middle);
            }
        }

        public override void Dispose()
        {
            if (_horizontalCenter)
            {
                _contentPane.RemoveRelation(_uiRoot, RelationType.Center_Center);
            }
        
            if (_verticalCenter)
            {
                _contentPane.RemoveRelation(_uiRoot, RelationType.Middle_Middle);
            }
        
            base.Dispose();
        }

        private void UpdatePosition()
        {
            float x = 0;
            float y = 0;
        
            if (_horizontalCenter)
            {
                x = (_uiRoot.width - _contentPane.width) / 2;
            }
        
            if (_verticalCenter)
            {
                y = (_uiRoot.height - _contentPane.height) / 2;
            }
        
            _contentPane.SetXY(x, y);
        }
    }
}