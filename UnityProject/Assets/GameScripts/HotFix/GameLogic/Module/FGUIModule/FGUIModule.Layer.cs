using System.Collections.Generic;
using FairyGUI;
using TEngine;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace GameLogic
{
    public enum FGUILayer
    {
        Background,
        Normal,
        Popup,
        Tip,
        Loading,
        TopMost
    }

    public partial class FGUIModule
    {
        private Dictionary<FGUILayer, GComponent> _layers = new Dictionary<FGUILayer, GComponent>();

        public void InitLayers()
        {
            foreach (FGUILayer layer in System.Enum.GetValues(typeof(FGUILayer)))
            {
                var comp = new GComponent();
                comp.name = layer.ToString();
                comp.gameObjectName = layer.ToString();
                comp.sortingOrder = (int)layer * 100;

                GRoot.inst.AddChild(comp);

                comp.AddRelation(GRoot.inst, RelationType.Width);
                comp.AddRelation(GRoot.inst, RelationType.Height);
                comp.MakeFullScreen();
                _layers.Add(layer, comp);
            }
        }


        public GComponent AddToLayer(GComponent view, FGUILayer layer)
        {
            GComponent parent = GetLayer(layer);
            parent.AddChild(view);
            view.SetSize(parent.width, parent.height);
            view.AddRelation(parent, RelationType.Size);
            return parent;
        }

        private GComponent GetLayer(FGUILayer layer)
        {
            return _layers[layer];
        }
    }
}