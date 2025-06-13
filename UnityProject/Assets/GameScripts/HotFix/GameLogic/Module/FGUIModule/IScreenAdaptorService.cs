using System;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 屏幕适配服务接口
    /// </summary>
    public interface IScreenAdaptorService
    {
        /// <summary>
        /// 安全区域范围（屏幕坐标）
        /// </summary>
        Rect SafeArea { get; }
    
        /// <summary>
        /// 安全区域变化事件
        /// </summary>
        event Action<Rect> OnSafeAreaChanged;
    
        /// <summary>
        /// 屏幕尺寸变化事件
        /// </summary>
        event Action<Vector2Int> OnScreenSizeChanged;
    }
}