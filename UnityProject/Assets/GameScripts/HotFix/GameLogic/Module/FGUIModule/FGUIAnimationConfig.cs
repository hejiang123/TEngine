using System;

namespace GameLogic
{
    public enum FGUIAnimationType
    {
        None,
        Fade,
        Scale,
        SlideFromLeft,
        SlideFromRight,
        SlideFromTop,
        SlideFromBottom,
        Custom 
    }
    public class FGUIAnimationConfig
    {
        public FGUIAnimationType ShowAnimation { get; set; } = FGUIAnimationType.Fade;
        public FGUIAnimationType HideAnimation { get; set; } = FGUIAnimationType.Fade;
        public float AnimationDuration { get; set; } = 0.3f;
        public Action CustomShowAnimation { get; set; }
        public Action CustomHideAnimation { get; set; }
    }
}