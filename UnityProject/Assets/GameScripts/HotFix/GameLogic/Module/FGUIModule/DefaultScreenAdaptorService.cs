using System;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 默认屏幕适配服务实现
    /// </summary>
    public class DefaultScreenAdaptorService : IScreenAdaptorService
    {
        private Vector2Int _lastScreenSize;
    
        public Rect SafeArea { get; private set; }
        public event Action<Rect> OnSafeAreaChanged;
        public event Action<Vector2Int> OnScreenSizeChanged;

        public DefaultScreenAdaptorService()
        {
            UpdateSafeArea();
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        
            // 每帧检查屏幕尺寸和安全区域变化
            //TickManager.Instance.AddUpdate(CheckScreenChange);
        }

        private void CheckScreenChange()
        {
            // 检查屏幕尺寸变化
            var currentSize = new Vector2Int(Screen.width, Screen.height);
            if (currentSize != _lastScreenSize)
            {
                _lastScreenSize = currentSize;
                OnScreenSizeChanged?.Invoke(currentSize);
                UpdateSafeArea();
            }
        
            // 检查安全区域变化
            var currentSafeArea = Screen.safeArea;
            if (currentSafeArea != SafeArea)
            {
                UpdateSafeArea();
            }
        }

        private void UpdateSafeArea()
        {
            SafeArea = Screen.safeArea;
            OnSafeAreaChanged?.Invoke(SafeArea);
        }
    }
}