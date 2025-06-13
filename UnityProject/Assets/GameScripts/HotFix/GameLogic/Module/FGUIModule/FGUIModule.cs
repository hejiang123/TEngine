using System;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public sealed partial class FGUIModule : Singleton<FGUIModule>,IUpdate
    {
        private readonly int _designResolutionX = 1650;
        private readonly int _designResolutionY = 750;

        private Dictionary<long, FGUIBase> _allWindows = new Dictionary<long, FGUIBase>();
        private Dictionary<long, FGUIBase> _showingWindows = new Dictionary<long, FGUIBase>();
        private Stack<long> _windowStack = new Stack<long>();
        private Queue<GTweenCallback> _pendingActions = new Queue<GTweenCallback>();
        
        
        public IScreenAdaptorService ScreenAdaptor { get; private set; }
        public FairyGUIScreenAdaptorService UIScreenAdaptor { get; private set; }
        public Rect UISafeArea => UIScreenAdaptor.UISafeArea;
        
        

        protected override void OnInit()
        {
            base.OnInit();

            UIObjectFactory.SetDefaultPackageItemExtension(FGUIHelper.GetComponent);
            GRoot.inst.SetContentScaleFactor(_designResolutionX, _designResolutionY, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
            
            ScreenAdaptor = new DefaultScreenAdaptorService();
            UIScreenAdaptor = new FairyGUIScreenAdaptorService(ScreenAdaptor);
            InitLayers();
        }

        public void OnUpdate()
        {
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            
            (ScreenAdaptor as IDisposable)?.Dispose();
            UIScreenAdaptor?.Dispose();
        }
        
        
        public T OpenWindow<T>(object param = null, FGUILayer layer = FGUILayer.Normal,
            bool allowMultiple = false) where T : FGUIBase, new()
        {
            FGUIConfig fguiConfig = null;
            Type type = typeof(T);
            var attributes = Attribute.GetCustomAttributes(type);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is FGUIConfigAttribute fguiConfigAttribute)
                {
                    string packageName = FGUIHelper.GetPackageNameById(fguiConfigAttribute.PackageId);
                    string componentName = typeof(T).Name;
                    string[] dependentPackageNames = null;
                    if (null != fguiConfigAttribute.DependentPackageIds)
                    {
                        dependentPackageNames = new string[fguiConfigAttribute.DependentPackageIds.Length];
                        for (int i = 0; i < fguiConfigAttribute.DependentPackageIds.Length; i++)
                        {
                            dependentPackageNames[i] = FGUIHelper.GetPackageNameById(fguiConfigAttribute.DependentPackageIds[i]);
                        }
                    }

                    fguiConfig = FGUIConfig.Create(packageName, componentName, dependentPackageNames);
                    break;
                }
            }

            if (null == fguiConfig)
            {
                throw new NullReferenceException("FGUIConfig is null");
            }

            fguiConfig.Layer = layer;

            // 不允许重复打开相同窗口
            if (!allowMultiple && _showingWindows.TryGetValue(fguiConfig.WindowId, out var window))
            {
                return window as T;
            }

            return OpenWindowWithConfig<T>(fguiConfig, param);
        }

        private T OpenWindowWithConfig<T>(FGUIConfig config, object param = null) where T : FGUIBase, new()
        {
            if (_allWindows.TryGetValue(config.WindowId, out FGUIBase window))
            {
                if (!window.IsShowing)
                {
                    ShowWindow(config.WindowId, window, param);
                }

                return window as T;
            }

            T newWindow = new T();
            newWindow.WindowId = config.WindowId;
            newWindow.View = CreateObject(newWindow.WindowId);
            if (newWindow.View == null)
            {
                Log.Error($"Create UI failed for windowId: {newWindow.WindowId}");
                return null;
            }
        
            newWindow.View.name = newWindow.WindowId.ToString();
            
            newWindow.View.visible = false;
            
            GComponent parent = AddToLayer(newWindow.View, config?.Layer ?? FGUILayer.Normal);
            newWindow.Parent = parent;
            newWindow.OnCreate();

            _allWindows[config.WindowId] = newWindow;
            ShowWindow(config.WindowId, newWindow, param);

            return newWindow;
        }

        private void ShowWindow(long windowId, FGUIBase window, object param = null)
        {
            if (window == null) return;

            // 恢复保存的状态
            if (FGUIConfig.GetConfig(windowId)?.SaveState == true)
            {
                var savedState = FGUIModule.Instance.GetWindowState(windowId);
                window.RestoreState(savedState);
            }

            window.BeforeShow(param);

            // 执行显示动画
            PlayShowAnimation(windowId, window, () =>
            {
                window.Show();
                window.AfterShow(param);
            });

            _showingWindows[windowId] = window;
            _windowStack.Push(windowId);
            
        }

        public void CloseWindow(long windowId, Action onComplete = null)
        {
            if (_showingWindows.TryGetValue(windowId, out FGUIBase window))
            {
                window.BeforeClose();

                // 保存状态
                if (FGUIConfig.GetConfig(windowId)?.SaveState == true)
                {
                    var state = window.SaveState();
                    FGUIModule.Instance.SaveWindowState(windowId, state);
                }

                // 执行关闭动画
                PlayHideAnimation(windowId, window, () =>
                {
                    window.Hide();
                    window.AfterClose();

                    _showingWindows.Remove(windowId);
                    RemoveFromStack(windowId);

                    var config = FGUIConfig.GetConfig(windowId);
                    if (config != null && !config.Cache)
                    {
                        DestroyWindow(windowId);
                    }

                    onComplete?.Invoke();
                });
            }
        }

        public void CloseAllWindows()
        {
            foreach (var windowId in _showingWindows.Keys.ToArray())
            {
                CloseWindow(windowId);
            }
        }

        private void PlayShowAnimation(long windowId, FGUIBase window, GTweenCallback onComplete)
        {
            var config = FGUIConfig.GetConfig(windowId);
            if (config == null || config.AnimationConfig.ShowAnimation == FGUIAnimationType.None)
            {
                onComplete?.Invoke();
                return;
            }

            var animConfig = config.AnimationConfig;
            window.View.visible = true;

            switch (animConfig.ShowAnimation)
            {
                case FGUIAnimationType.Fade:
                    window.View.alpha = 0;
                    window.View.TweenFade(1, animConfig.AnimationDuration).OnComplete(onComplete);
                    break;

                case FGUIAnimationType.Scale:
                    window.View.SetScale(0.5f, 0.5f);
                    window.View.TweenScale(new Vector2(1, 1), animConfig.AnimationDuration)
                        .SetEase(EaseType.BackOut)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromLeft:
                    window.View.x = -window.View.width;
                    window.View.TweenMoveX(0, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadOut)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromRight:
                    window.View.x = GRoot.inst.width;
                    window.View.TweenMoveX(GRoot.inst.width - window.View.width, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadOut)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromTop:
                    window.View.y = -window.View.height;
                    window.View.TweenMoveY(0, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadOut)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromBottom:
                    window.View.y = GRoot.inst.height;
                    window.View.TweenMoveY(GRoot.inst.height - window.View.height, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadOut)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.Custom:
                    animConfig.CustomShowAnimation?.Invoke();
                    _pendingActions.Enqueue(onComplete);
                    break;

                default:
                    onComplete?.Invoke();
                    break;
            }
        }

        private void PlayHideAnimation(long windowId, FGUIBase window, GTweenCallback onComplete)
        {
            var config = FGUIConfig.GetConfig(windowId);
            if (config == null || config.AnimationConfig.HideAnimation == FGUIAnimationType.None)
            {
                onComplete?.Invoke();
                return;
            }

            var animConfig = config.AnimationConfig;
            window.View.visible = false;

            switch (animConfig.HideAnimation)
            {
                case FGUIAnimationType.Fade:
                    window.View.TweenFade(0, animConfig.AnimationDuration).OnComplete(onComplete);
                    break;

                case FGUIAnimationType.Scale:
                    window.View.TweenScale(new Vector2(0.5f, 0.5f), animConfig.AnimationDuration)
                        .SetEase(EaseType.BackIn)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromLeft:
                    window.View.TweenMoveX(-window.View.width, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadIn)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromRight:
                    window.View.TweenMoveX(GRoot.inst.width, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadIn)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromTop:
                    window.View.TweenMoveY(-window.View.height, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadIn)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.SlideFromBottom:
                    window.View.TweenMoveY(GRoot.inst.height, animConfig.AnimationDuration)
                        .SetEase(EaseType.QuadIn)
                        .OnComplete(onComplete);
                    break;

                case FGUIAnimationType.Custom:
                    animConfig.CustomHideAnimation?.Invoke();
                    _pendingActions.Enqueue(onComplete);
                    break;

                default:
                    onComplete?.Invoke();
                    break;
            }
        }

        public void BackToPreviousWindow(Action onNoMoreWindows = null)
        {
            if (_windowStack.Count > 0)
            {
                long current = _windowStack.Pop();

                if (_windowStack.Count > 0)
                {
                    long previous = _windowStack.Peek();
                    if (_allWindows.TryGetValue(previous, out FGUIBase window))
                    {
                        window.Show();
                    }
                }
                else
                {
                    onNoMoreWindows?.Invoke();
                }

                CloseWindow(current);
            }
            else
            {
                onNoMoreWindows?.Invoke();
            }
        }


        private void DestroyWindow(long windowId)
        {
            if (_allWindows.TryGetValue(windowId, out FGUIBase window))
            {
                window.Dispose();
                _allWindows.Remove(windowId);

                var config = FGUIConfig.GetConfig(windowId);
                if (config != null)
                {
                    FGUIModule.Instance.UnloadUIPackage(config.PackageName);
                    if (null != config.DependentPackageNames)
                    {
                        foreach (var packageName in config.DependentPackageNames)
                        {
                            FGUIModule.Instance.UnloadUIPackage(packageName);
                        }
                    }

                    FGUIModule.Instance.ClearWindowState(windowId);
                }
            }
        }

        private void RemoveFromStack(long windowId)
        {
            if (_windowStack.Count == 0) return;

            Stack<long> tempStack = new Stack<long>();
            bool found = false;

            while (_windowStack.Count > 0)
            {
                long id = _windowStack.Pop();
                if (id != windowId)
                {
                    tempStack.Push(id);
                }
                else
                {
                    found = true;
                    break;
                }
            }

            while (tempStack.Count > 0)
            {
                _windowStack.Push(tempStack.Pop());
            }

            if (!found)
            {
                Log.Warning($"WindowId {windowId} not found in stack");
            }
        }
    }
}