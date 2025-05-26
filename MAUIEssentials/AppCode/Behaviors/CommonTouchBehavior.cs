using CommunityToolkit.Maui.Behaviors;

namespace MAUIEssentials.AppCode.Behaviors
{
    public class CommonTouchBehavior : TouchBehavior
    {
        public CommonTouchBehavior()
        {
            DefaultAnimationDuration = 100;
            DefaultAnimationEasing = Easing.CubicInOut;
            PressedOpacity = 0.4;
        }
    }
}
