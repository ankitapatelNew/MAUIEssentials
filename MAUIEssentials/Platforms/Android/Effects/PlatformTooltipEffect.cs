using MAUIEssentials.AppCode.Behaviors;
using Microsoft.Maui.Controls.Platform;

namespace MAUIEssentials.Effects
{
    public class PlatformTooltipEffect : PlatformEffect
    {
        //Tooltip tooltip;
        ControlTooltipEffect effect;

        public PlatformTooltipEffect()
        {
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;
            control.Click += OnTap;

            effect = (ControlTooltipEffect)Element.Effects.FirstOrDefault(e => e is ControlTooltipEffect);
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.Click -= OnTap;
            //tooltip?.Dismiss();
        }

        void OnTap(object sender, EventArgs e)
        {
            var control = Control ?? Container;
            effect.OnTooltipCalled(Element);

            var text = TooltipEffect.GetText(Element);
            //tooltip?.Dismiss();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var parentContent = control.RootView;
            var position = TooltipEffect.GetPosition(Element);

            //var closeBuilder = new ClosePolicy.Builder()
            //    .Inside(true)
            //    .Outside(TooltipEffect.GetOuterClickEnable(Element))
            //    .Build();

            //Tooltip.Gravity gravity;
            //switch (position)
            //{
            //    case TooltipPosition.Top:
            //        gravity = Tooltip.Gravity.Top;
            //        break;
            //    case TooltipPosition.Left:
            //        gravity = Tooltip.Gravity.Left;
            //        break;
            //    case TooltipPosition.Right:
            //        gravity = Tooltip.Gravity.Right;
            //        break;
            //    default:
            //        gravity = Tooltip.Gravity.Bottom;
            //        break;
            //}

            //tooltip = new Tooltip.Builder(control.Context)
            //    .Anchor(control, 0, 0, false)
            //    .Arrow(true)
            //    .ClosePolicy(closeBuilder)
            //    .FloatingAnimation(null)
            //    .ShowDuration(0)
            //    .Overlay(false)
            //    .MaxWidth(Platform.CurrentActivity.Resources.DisplayMetrics.WidthPixels / 2)
            //    .Text(text)
            //    //.StyleId(Java.Lang.Integer.ValueOf(Resource.Style.ToolTipAltStyle))
            //    .Create();

            //tooltip?.Show(parentContent, gravity, true);
        }
    }
}
