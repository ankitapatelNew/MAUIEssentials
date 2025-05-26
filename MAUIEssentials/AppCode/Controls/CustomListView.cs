using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using AsyncAwaitBestPractices;

namespace MAUIEssentials.AppCode.Controls
{
    public class CustomListView : ListView
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event Action<int> ReloadRow;

        public event Action<int> EventScrollToGroup;
        public event EventHandler EventScrollToTop;

        readonly WeakEventManager<SwipedEventArgs> swipeEventManager = new WeakEventManager<SwipedEventArgs>();
        readonly AsyncAwaitBestPractices.WeakEventManager draggedEventManager = new AsyncAwaitBestPractices.WeakEventManager();

        public event EventHandler Dragged
        {
            add => draggedEventManager.AddEventHandler(value);
            remove => draggedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<SwipedEventArgs> OnSwipe
        {
            add => swipeEventManager.AddEventHandler(value);
            remove => swipeEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<int> FirstVisibleItemIndexChanged;

        public void NotifyFirstVisibleItemIndexChanged(int index)
        {
            FirstVisibleItemIndexChanged?.Invoke(this, index);
        }

        public static readonly BindableProperty IsScrollDisabledProperty =
            BindableProperty.Create(nameof(IsScrollDisabled), typeof(bool), typeof(CustomListView), false);

        public static readonly BindableProperty DisableSelectionProperty =
            BindableProperty.Create(nameof(DisableSelection), typeof(bool), typeof(CustomListView), false);

        public static readonly BindableProperty LoadMoreCommandProperty =
            BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(CustomListView), default(ICommand));

        public static readonly BindableProperty DisableBounceProperty =
            BindableProperty.Create(nameof(DisableBounce), typeof(bool), typeof(CustomListView), false);

        public void OnSwiped(SwipeDirection direction) =>
            swipeEventManager?.RaiseEvent(this, new SwipedEventArgs(null, direction), nameof(OnSwipe));

        public CustomListView()
        {
            ItemAppearing -= CustomListView_ItemAppearing;
            ItemAppearing += CustomListView_ItemAppearing;
        }

        void CustomListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = ItemsSource as IList;

            if (items?.Count > 0 && e.Item == items[items.Count - 1])
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
            draggedEventManager?.RaiseEvent(this, EventArgs.Empty, nameof(Dragged));
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
        
        public Func<int> NativeFirstVisbileIndex { get; set; }
        public int GetFirstVisbileIndex()
        {
            if (NativeFirstVisbileIndex != null)
            {
                return NativeFirstVisbileIndex();
            }
            else
            {
                return 0;
            }
        }
    }
}