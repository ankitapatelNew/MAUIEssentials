using System.ComponentModel;

namespace MAUIEssentials.AppCode.Controls
{
     public class CustomTabbedPage : TabbedPage
    {
        readonly AsyncAwaitBestPractices.WeakEventManager pagechangeEventManager = new AsyncAwaitBestPractices.WeakEventManager();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler OnPageChanged
        {
            add => pagechangeEventManager.AddEventHandler(value);
            remove => pagechangeEventManager.RemoveEventHandler(value);
        }

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(CustomTabbedPage), string.Empty);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CustomTabbedPage), 14d);

        public static readonly BindableProperty ShowTitleOnSelectedProperty =
            BindableProperty.Create(nameof(ShowTitleOnSelected), typeof(bool), typeof(CustomTabbedPage), false);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public bool ShowTitleOnSelected
        {
            get => (bool)GetValue(ShowTitleOnSelectedProperty);
            set => SetValue(ShowTitleOnSelectedProperty, value);
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            pagechangeEventManager?.RaiseEvent(this, EventArgs.Empty, nameof(OnPageChanged));
        }
    }
}
