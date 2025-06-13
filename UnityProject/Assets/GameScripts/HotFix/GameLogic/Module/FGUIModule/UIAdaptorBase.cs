using FairyGUI;

namespace GameLogic
{
    /// <summary>
    /// UI适配器基类
    /// </summary>
    public class UIAdaptorBase
    {
        protected GComponent _contentPane;
        protected GComponent _uiRoot;
    
        /// <summary>
        /// 初始化适配器
        /// </summary>
        public virtual void Initialize(GComponent contentPane, GComponent uiRoot)
        {
            _contentPane = contentPane;
            _uiRoot = uiRoot;
            
            //GRoot.inst.onSizeChanged.Add(HandleSizeChanged);
        }
    
        /// <summary>
        /// 释放适配器
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}