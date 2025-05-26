using System;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Behaviors
{
    public class TooltipEffect : RoutingEffect
    {
        public static Action<object>? TooltipAction { get; set; }

        public static readonly BindableProperty HasTooltipProperty =
            BindableProperty.CreateAttached("HasTooltip", typeof(bool), typeof(TooltipEffect), false, propertyChanged: OnHasTooltipChanged);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.CreateAttached("TextColor", typeof(Color), typeof(TooltipEffect), Colors.White);

        public static readonly BindableProperty BackgroundColorProperty =
            BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(TooltipEffect), Colors.Black);

        public static readonly BindableProperty TextProperty =
            BindableProperty.CreateAttached("Text", typeof(string), typeof(TooltipEffect), string.Empty);

        public static readonly BindableProperty PositionProperty =
            BindableProperty.CreateAttached("Position", typeof(TooltipPosition), typeof(TooltipEffect), TooltipPosition.Bottom);

        public static readonly BindableProperty OuterClickEnableProperty =
            BindableProperty.CreateAttached("OuterClickEnable", typeof(bool), typeof(TooltipEffect), true);

        public static bool GetHasTooltip(BindableObject view)
        {
            return (bool)view.GetValue(HasTooltipProperty);
        }

        public static void SetHasTooltip(BindableObject view, bool value)
        {
            view.SetValue(HasTooltipProperty, value);
        }

        public static Color GetTextColor(BindableObject view)
        {
            return (Color)view.GetValue(TextColorProperty);
        }

        public static void SetTextColor(BindableObject view, Color value)
        {
            view.SetValue(TextColorProperty, value);
        }

        public static Color GetBackgroundColor(BindableObject view)
        {
            return (Color)view.GetValue(BackgroundColorProperty);
        }

        public static void SetBackgroundColor(BindableObject view, Color value)
        {
            view.SetValue(BackgroundColorProperty, value);
        }

        public static string GetText(BindableObject view)
        {
            return (string)view.GetValue(TextProperty);
        }

        public static void SetText(BindableObject view, string value)
        {
            view.SetValue(TextProperty, value);
        }

        public static bool GetOuterClickEnable(BindableObject view)
        {
            return (bool)view.GetValue(OuterClickEnableProperty);
        }

        public static void SetOuterClickEnable(BindableObject view, bool value)
        {
            view.SetValue(OuterClickEnableProperty, value);
        }

        public static TooltipPosition GetPosition(BindableObject view)
        {
            return (TooltipPosition)view.GetValue(PositionProperty);
        }

        public static void SetPosition(BindableObject view, TooltipPosition value)
        {
            view.SetValue(PositionProperty, value);
        }

        static void OnHasTooltipChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }

            bool hasTooltip = (bool)newValue;
            if (hasTooltip)
            {
                var effect = new ControlTooltipEffect();
                effect.TooltipTapped += Effect_TooltipTapped;

                view.Effects.Add(effect);
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is ControlTooltipEffect);
                if (toRemove != null)
                {
                    (toRemove as ControlTooltipEffect).TooltipTapped -= Effect_TooltipTapped;
                    view.Effects.Remove(toRemove);
                }
            }
        }

        private static void Effect_TooltipTapped(object sender, EventArgs e)
        {
            TooltipAction?.Invoke(sender);
        }
    }

    public class ControlTooltipEffect : RoutingEffect
    {
        public event EventHandler<EventArgs>? TooltipTapped;

        public ControlTooltipEffect() : base($"MAUIEssentials.{nameof(TooltipEffect)}")
        {

        }

        public void OnTooltipCalled(object sender)
        {
            TooltipTapped?.Invoke(sender, new EventArgs());
        }
    }
}