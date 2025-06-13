using System.Collections.Generic;
using FairyGUI;

namespace GameLogic
{
    public class FGUIConfig
    {
        private static Dictionary<long, FGUIConfig> _configCache = new Dictionary<long, FGUIConfig>();
        
        public long WindowId { get; private set; }
        public string PackageName { get; private set; }
        public string[] DependentPackageNames { get; private set; }
        public string ComponentName { get; private set; }
        public FGUILayer Layer { get; set; } = FGUILayer.Normal;
        public bool Cache { get; set; } = false;
        public bool SaveState { get; set; } = false;
        public FGUIAnimationConfig AnimationConfig { get; set; } = new FGUIAnimationConfig();
        public object DefaultData { get; set; }
        
        private FGUIConfig(long windowId, string packageName, string componentName, string[] dependentPackageNames)
        {
            WindowId = windowId;
            PackageName = packageName;
            ComponentName = componentName;
            DependentPackageNames = dependentPackageNames;
        }
        
        public static FGUIConfig Create(string packageName, string componentName,string[] dependentPackageNames)
        {
            long windowId = GenerateWindowId(packageName, componentName);
        
            if (_configCache.TryGetValue(windowId, out FGUIConfig config))
            {
                return config;
            }
        
            config = new FGUIConfig(windowId, packageName, componentName,dependentPackageNames);
            _configCache[windowId] = config;
            return config;
        }
        
        private static long GenerateWindowId(string packageName, string componentName)
        {
            string key = $"{packageName.ToLower()}_{componentName.ToLower()}";
            const ulong fnvOffsetBasis = 14695981039346656037;
            const ulong fnvPrime = 1099511628211;
        
            ulong hash = fnvOffsetBasis;
            foreach (byte b in System.Text.Encoding.UTF8.GetBytes(key))
            {
                hash ^= b;
                hash *= fnvPrime;
            }
        
            return (long)(hash & 0x7FFFFFFFFFFFFFFF);
        }
        
        public static FGUIConfig GetConfig(long windowId)
        {
            _configCache.TryGetValue(windowId, out FGUIConfig config);
            return config;
        }
    }
}