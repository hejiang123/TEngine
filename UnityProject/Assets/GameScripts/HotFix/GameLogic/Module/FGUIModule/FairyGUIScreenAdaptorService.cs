using System;
using FairyGUI;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// FairyGUI UI适配服务
    /// </summary>
    public class FairyGUIScreenAdaptorService : IDisposable
    {
        private readonly IScreenAdaptorService _screenAdaptor;
    
    /// <summary>
    /// UI屏幕尺寸（逻辑尺寸）
    /// </summary>
    public Vector2Int UIScreenSize { get; private set; }
    
    /// <summary>
    /// UI安全区域（逻辑尺寸）
    /// </summary>
    public Rect UISafeArea { get; private set; }
    
    /// <summary>
    /// UI屏幕尺寸变化事件
    /// </summary>
    public event Action<Vector2Int> OnUIScreenSizeChanged;
    
    /// <summary>
    /// UI安全区域变化事件
    /// </summary>
    public event Action<Rect> OnUISafeAreaChanged;

    public FairyGUIScreenAdaptorService(IScreenAdaptorService screenAdaptor)
    {
        _screenAdaptor = screenAdaptor;
        
        // 初始化监听
        _screenAdaptor.OnSafeAreaChanged += HandleSafeAreaChanged;
        _screenAdaptor.OnScreenSizeChanged += HandleScreenSizeChanged;
        //GRoot.inst.onSizeChanged += HandleUISizeChanged;
        
        // 初始计算
        UpdateAll();
    }

    public void Dispose()
    {
        _screenAdaptor.OnSafeAreaChanged -= HandleSafeAreaChanged;
        _screenAdaptor.OnScreenSizeChanged -= HandleScreenSizeChanged;
        //GRoot.inst.onSizeChanged -= HandleUISizeChanged;
    }

    private void HandleSafeAreaChanged(Rect newSafeArea)
    {
        UpdateUISafeArea();
    }

    private void HandleScreenSizeChanged(Vector2Int newSize)
    {
        UpdateAll();
    }

    private void HandleUISizeChanged()
    {
        UpdateAll();
    }

    private void UpdateAll()
    {
        UpdateUIScreenSize();
        UpdateUISafeArea();
    }

    private void UpdateUIScreenSize()
    {
        UIScreenSize = new Vector2Int(
            Mathf.CeilToInt(GRoot.inst.width),
            Mathf.CeilToInt(GRoot.inst.height)
        );
        OnUIScreenSizeChanged?.Invoke(UIScreenSize);
    }

    private void UpdateUISafeArea()
    {
        /*var screenSafeArea = _screenAdaptor.SafeArea;
        var scaleFactor = 1f / GRoot.contentScaleFactor;
        
        UISafeArea = new Rect(
            Mathf.CeilToInt(screenSafeArea.x * scaleFactor),
            Mathf.CeilToInt(screenSafeArea.y * scaleFactor),
            Mathf.CeilToInt(screenSafeArea.width * scaleFactor),
            Mathf.CeilToInt(screenSafeArea.height * scaleFactor)
        );*/
        UISafeArea = GRoot.inst.GlobalToLocal(_screenAdaptor.SafeArea);
        OnUISafeAreaChanged?.Invoke(UISafeArea);
    }
    }
}