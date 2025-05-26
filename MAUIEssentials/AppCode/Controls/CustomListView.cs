using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

namespace MAUIEssentials.AppCode.Controls
{
    public class CustomListView : ListView
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event Action<int> ReloadRow;

        public event Action<int> EventScrollToGroup;
        public event EventHandler EventScrollToTop;
        public event EventHandler Dragged;

        public static readonly BindableProperty IsScrollDisabledProperty =
            BindableProperty.Create(nameof(IsScrollDisabled), typeof(bool), typeof(CustomListView), false);

        public static readonly BindableProperty DisableSelectionProperty =
            BindableProperty.Create(nameof(DisableSelection), typeof(bool), typeof(CustomListView), false);

        public static readonly BindableProperty LoadMoreCommandProperty =
            BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(CustomListView), default(ICommand));

        public static readonly BindableProperty DisableBounceProperty =
            BindableProperty.Create(nameof(DisableBounce), typeof(bool), typeof(CustomListView), false);

        public CustomListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            ItemAppearing += CustomListView_ItemAppearing;
        }

        void CustomListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = ItemsSource as IList;

            if (items != null && e.Item == items[items.Count - 1])
            {
                if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                    LoadMoreCommand.Execute(null);
            }
        }

        public void ScrollToTop(bool animate = true)
        {
            EventScrollToTop?.Invoke(this, EventArgs.Empty);
        }

        public void ScrollToGroup(int groupPosition, int itemPosition)
        {
            EventScrollToGroup?.Invoke(groupPosition + itemPosition);
        }

        public void OnDragged()
        {
            Dragged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsScrollDisabled
        {
            get => (bool)GetValue(IsScrollDisabledProperty);
            set => SetValue(IsScrollDisabledProperty, value);
        }

        public bool DisableSelection
        {
            get => (bool)GetValue(DisableSelectionProperty);
            set => SetValue(DisableSelectionProperty, value);
        }

        public ICommand LoadMoreCommand
        {
            get => (ICommand)GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        public bool DisableBounce
        {
            get => (bool)GetValue(DisableBounceProperty);
            set => SetValue(DisableBounceProperty, value);
        }

        public void ReloadRowData(int index)
        {
            ReloadRow?.Invoke(index);
        }
    }
}