using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace MAUIEssentials.AppCode.Controls
{
    public class BindableStackLayout : StackLayout
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(BindableStackLayout),
                        propertyChanged: (bindable, oldValue, newValue) => ((BindableStackLayout)bindable).PopulateItems(),
                        defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty ItemDataTemplateProperty =
            BindableProperty.Create(nameof(ItemDataTemplate), typeof(DataTemplate), typeof(BindableStackLayout));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemDataTemplate
        {
            get { return (DataTemplate)GetValue(ItemDataTemplateProperty); }
            set { SetValue(ItemDataTemplateProperty, value); }
        }

        void PopulateItems()
        {
            if (ItemsSource == null) return;

            if (Children != null)
            {
                Children.Clear();
            }

            foreach (var item in ItemsSource)
            {
                var itemTemplate = ItemDataTemplate.CreateContent() as View;
                itemTemplate.BindingContext = item;
                Children.Add(itemTemplate);
            }

        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(ItemsSource):
                    if (ItemsSource != null && ItemsSource is INotifyCollectionChanged collection)
                    {
                        collection.CollectionChanged -= Collection_CollectionChanged;
                        collection.CollectionChanged += Collection_CollectionChanged;
                    }
                    break;
            }
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var itemTemplate = ItemDataTemplate.CreateContent() as View;
                        itemTemplate.BindingContext = item;
                        Children.Add(itemTemplate);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (Children.Count > 0)
                    {
                        Children.RemoveAt(e.OldStartingIndex);
                    }
                    break;
            }
        }
    }
}