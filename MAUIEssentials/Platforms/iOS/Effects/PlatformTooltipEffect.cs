using System.ComponentModel;
using MAUIEssentials.AppCode.Behaviors;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Models;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.Effects
{
    public class PlatformTooltipEffect : PlatformEffect
    {
        //EasyTipView.EasyTipView tooltip;
        UITapGestureRecognizer tapGestureRecognizer;
        ControlTooltipEffect effect;

        public PlatformTooltipEffect()
        {
            //tooltip = new EasyTipView.EasyTipView();
            //tooltip.DidDismiss += OnDismiss;
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            if (control is UIButton)
            {
                var btn = Control as UIButton;
                btn.TouchUpInside += OnTap;

            }
            else
            {
                tapGestureRecognizer = new UITapGestureRecognizer((UITapGestureRecognizer obj) => {
                    OnTap(obj, EventArgs.Empty);
                });
                control.UserInteractionEnabled = true;
                control.AddGestureRecognizer(tapGestureRecognizer);
            }

            effect = (ControlTooltipEffect)Element.Effects.FirstOrDefault(e => e is ControlTooltipEffect);
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;

            if (control is UIButton)
            {
                var btn = Control as UIButton;
                btn.TouchUpInside -= OnTap;

            }
            else
            {
                if (tapGestureRecognizer != null)
                    control.RemoveGestureRecognizer(tapGestureRecognizer);
            }
            //tooltip?.Dismiss();
        }

        void OnTap(object sender, EventArgs e)
        {
            if (Element == null)
            {
                return;
            }
            var control = Control ?? Container;
            effect.OnTooltipCalled(Element);

            var text = TooltipEffect.GetText(Element);

            var font = Settings.AppLanguage?.Language == AppLanguage.Arabic
                ? "Noto Naskh Arabic" : "Roboto";

            if (!string.IsNullOrEmpty(text))
            {
                //tooltip.BubbleColor = TooltipEffect.GetBackgroundColor(Element).ToUIColor();
                //tooltip.ForegroundColor = TooltipEffect.GetTextColor(Element).ToUIColor();
                //tooltip.Text = new Foundation.NSString(text);
                //tooltip.Font = UIFont.FromName(font, 15);
                UpdatePosition();

                var window = UIApplication.SharedApplication?.KeyWindow;
                var vc = window?.RootViewController;
                while (vc?.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                //tooltip?.Show(control, vc.View, true);
            }
        }

        void OnDismiss(object sender, EventArgs e)
        {
            // do something on dismiss
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            //if (args.PropertyName == TooltipEffect.BackgroundColorProperty.PropertyName)
            //{
            //    tooltip.BubbleColor = TooltipEffect.GetBackgroundColor(Element).ToUIColor();
            //}
            //else if (args.PropertyName == TooltipEffect.TextColorProperty.PropertyName)
            //{
            //    tooltip.ForegroundColor = TooltipEffect.GetTextColor(Element).ToUIColor();
            //}
            //else if (args.PropertyName == TooltipEffect.TextProperty.PropertyName)
            //{
            //    tooltip.Text = new Foundation.NSString(TooltipEffect.GetText(Element));
            //}
            //else if (args.PropertyName == TooltipEffect.PositionProperty.PropertyName)
            //{
            //    UpdatePosition();
            //}
        }

        void UpdatePosition()
        {
            var position = TooltipEffect.GetPosition(Element);
            switch (position)
            {
                //case TooltipPosition.Top:
                //    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Bottom;
                //    break;
                //case TooltipPosition.Left:
                //    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Right;
                //    break;
                //case TooltipPosition.Right:
                //    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Left;
                //    break;
                //default:
                //    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Top;
                //    break;
            }
        }
    }
}
