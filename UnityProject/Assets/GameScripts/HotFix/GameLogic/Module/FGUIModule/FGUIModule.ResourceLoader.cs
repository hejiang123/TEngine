using System;
using System.Collections.Generic;
using FairyGUI;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public partial class FGUIModule
    {
        private Dictionary<string, UIPackage> _loadedPackages = new Dictionary<string, UIPackage>();
        private Dictionary<string, int> _packageRefCount = new Dictionary<string, int>();
        private Dictionary<long, object> _windowStates = new Dictionary<long, object>();
        private Dictionary<string,List<object>> _packageAssets = new Dictionary<string, List<object>>();
        
        public void LoadUIPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) return;
        
            if (_packageRefCount.ContainsKey(packageName))
            {
                _packageRefCount[packageName]++;
                return;
            }
        
            UIPackage pkg = UIPackage.AddPackage(packageName, ((string name, string extension, Type type, out DestroyMethod method) =>
            {
                method = DestroyMethod.None;

                if (!_packageAssets.TryGetValue(packageName, out List<object> list))
                {
                    list = new List<object>();
                    _packageAssets.TryAdd(packageName, list);
                }

                if (nameof(TextAsset) == type.Name)
                {
                    object textAsset = GameModule.Resource.LoadAsset<TextAsset>($"Assets/AssetRaw/FGUI/{name}");
                    list.Add(textAsset);
                    return textAsset;
                }
                
                Log.Error($"加载资源 {name} {extension} {type.Name} {nameof(TextAsset)}");
                return null;
            }));
            _loadedPackages[packageName] = pkg;
            _packageRefCount[packageName] = 1;
        }
        
        public void UnloadUIPackage(string packageName)
        {
            if (!_packageRefCount.ContainsKey(packageName)) return;
        
            _packageRefCount[packageName]--;
        
            if (_packageRefCount[packageName] <= 0)
            {
                UIPackage.RemovePackage(packageName);
                _loadedPackages.Remove(packageName);
                _packageRefCount.Remove(packageName);
                if (_packageAssets.TryGetValue(packageName, out List<object> list))
                {
                    foreach (var asset in list)
                    {
                        GameModule.Resource.UnloadAsset(asset);
                    }   
                    _packageAssets.Remove(packageName);
                }
            }
        }
        
        public GComponent CreateObject(long windowId)
        {
            var config = FGUIConfig.GetConfig(windowId);
            if (config == null)
            {
                Log.Error($"UIConfig not found for windowId: {windowId}");
                return null;
            }
        
            LoadUIPackage(config.PackageName);
            if (null != config.DependentPackageNames)
            {
                foreach (var packageName in config.DependentPackageNames)
                {
                    LoadUIPackage(packageName);
                }
            }
            return UIPackage.CreateObject(config.PackageName, config.ComponentName).asCom;
        }
        
        // 保存窗口状态
        public void SaveWindowState(long windowId, object state)
        {
            if (FGUIConfig.GetConfig(windowId)?.SaveState == true)
            {
                _windowStates[windowId] = state;
            }
        }
    
        // 获取窗口状态
        public object GetWindowState(long windowId)
        {
            _windowStates.TryGetValue(windowId, out object state);
            return state;
        }
    
        // 清除窗口状态
        public void ClearWindowState(long windowId)
        {
            if (_windowStates.ContainsKey(windowId))
            {
                _windowStates.Remove(windowId);
            }
        }

    }
}