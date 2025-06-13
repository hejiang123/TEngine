using FairyGUI;
using UnityEngine;

namespace GameLogic
{
    public partial class FGUIModule
    {
        private bool _enabled = true;
        private Color _color = Color.black;

        private GGraph _leftBar;
        private GGraph _rightBar;
        private GGraph _topBar;
        private GGraph _bottomBar;
        private GComponent _container;
        private float _lastAspectRatio;

        public void UpdateBlackBars()
        {
            if (!_enabled) return;

            
            var safeArea = GRoot.inst.GlobalToLocal(Screen.safeArea);
            
            
            _lastAspectRatio = CalculateScreenAspectRatio();
            float designAspect = GRoot.inst.width / GRoot.inst.height;
            float screenAspect = _lastAspectRatio;

            EnsureContainerExists();

            if (Mathf.Approximately(screenAspect, designAspect))
            {
                HideAllBars();
                return;
            }

            if (screenAspect > designAspect) // Wider than design (top/bottom bars)
            {
                float visibleHeight = GRoot.inst.width / screenAspect;
                float barHeight = (GRoot.inst.height - visibleHeight) / 2f;

                UpdateBar(_topBar, 0, 0, GRoot.inst.width, barHeight);
                UpdateBar(_bottomBar, 0, GRoot.inst.height - barHeight, GRoot.inst.width, barHeight);
                _leftBar.visible = _rightBar.visible = false;
            }
            else // Taller than design (left/right bars)
            {
                float visibleWidth = GRoot.inst.height * screenAspect;
                float barWidth = (GRoot.inst.width - visibleWidth) / 2f;

                UpdateBar(_leftBar, 0, 0, barWidth, GRoot.inst.height);
                UpdateBar(_rightBar, GRoot.inst.width - barWidth, 0, barWidth, GRoot.inst.height);
                _topBar.visible = _bottomBar.visible = false;
            }
        }

        private void EnsureContainerExists()
        {
            if (_container != null) return;

            _container = new GComponent
            {
                gameObjectName = "BlackBarsContainer",
                touchable = false
            };
            _container.SetSize(GRoot.inst.width, GRoot.inst.height);
            _container.AddRelation(GRoot.inst, RelationType.Size);

            _leftBar = CreateBar();
            _rightBar = CreateBar();
            _topBar = CreateBar();
            _bottomBar = CreateBar();

            _container.AddChild(_leftBar);
            _container.AddChild(_rightBar);
            _container.AddChild(_topBar);
            _container.AddChild(_bottomBar);

            GRoot.inst.AddChildAt(_container, 0);
        }

        private GGraph CreateBar()
        {
            var bar = new GGraph { touchable = false };

            /*
            if (!string.IsNullOrEmpty(_packageName) && !string.IsNullOrEmpty(_componentName))
            {
                try
                {
                    var comp = UIPackage.CreateObject(_packageName, _componentName);
                    if (comp != null)
                    {
                        bar.SetNativeObject(comp.displayObject);
                        return bar;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to create custom black bar: {e.Message}");
                }
            }*/

            // Fallback to colored rectangle
            bar.DrawRect(1, 1, 0, Color.clear, _color);
            return bar;
        }

        private void UpdateBar(GGraph bar, float x, float y, float width, float height)
        {
            if (width <= 0 || height <= 0)
            {
                bar.visible = false;
                return;
            }

            bar.visible = true;
            bar.SetXY(x, y);
            bar.SetSize(width, height);

            // Redraw if using simple rectangle
            if (bar.displayObject.graphics != null)
            {
                bar.DrawRect(width, height, 0, Color.clear, _color);
            }
        }

        private void HideAllBars()
        {
            if (_leftBar != null) _leftBar.visible = false;
            if (_rightBar != null) _rightBar.visible = false;
            if (_topBar != null) _topBar.visible = false;
            if (_bottomBar != null) _bottomBar.visible = false;
        }

        private float CalculateScreenAspectRatio()
        {
            return (float)Screen.width / Screen.height;
        }
    }
}