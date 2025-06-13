using FairyGUI;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public abstract class BindedFGUIBase<UIView> : FGUIBase where UIView : GComponent
    {
        public UIView UI { get { return (UIView)View; } }
    }

    
    public abstract class FGUIBase
    {
        public long WindowId;
        public GComponent View;
        public bool IsShowing { get; protected set; }
        public GComponent Parent;


        public virtual void OnCreate()
        {
            InitListeners();
        }
        
        protected abstract void InitListeners();
    
        public virtual void Show()
        {
            IsShowing = true;
        }
    
        public virtual void Hide()
        {
            IsShowing = false;
        }
        
        public virtual void OnScreenResized()
        {
            var config = FGUIConfig.GetConfig(WindowId);
            if (config != null)
            {
                if (config.Layer == FGUILayer.Normal || config.Layer == FGUILayer.Popup)
                {
                    View.Center();
                }
            }
        }
        
        public virtual object SaveState()
        {
            return null;
        }
        
        public virtual void RestoreState(object state)
        {
        }
        
        public virtual void BeforeShow(object param = null) { }
        public virtual void AfterShow(object param = null) { }
        public virtual void BeforeClose() { }
        public virtual void AfterClose() { }
    
        public virtual void Dispose()
        {
            if (View != null)
            {
                View.Dispose();
                View = null;
            }
        }
    }
}