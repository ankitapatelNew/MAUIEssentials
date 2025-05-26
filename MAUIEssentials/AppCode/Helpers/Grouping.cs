using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace MAUIEssentials.AppCode.Helpers
{
    public class Grouping<K, T> : ObservableCollection<T>, INotifyPropertyChanged
    {
        bool _expanded, _disableOnCollectionChanged;
        readonly IEnumerable<T> Data;

        public K Key { get; private set; }
        public int ColumnCount { get; private set; }
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

        public Grouping(K key)
        {
            Key = key;
        }

        public Grouping(K key, int columnCount) : this(key)
        {
            ColumnCount = columnCount;
        }

        public Grouping(K key, int columnCount, IEnumerable<T> items, bool expanded = false) : this(key, columnCount)
        {
            Data = items;
            Expanded = expanded;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Expanded
        {
            get => _expanded;
            set
            {
                _expanded = value;
                OnPropertyChanged(nameof(Expanded));
                UpdateData();
            }
        }

        public void UpdateData()
        {
            if (Data == null)
            {
                return;
            }

            if (Expanded)
            {
                AddRange(Data);
            }
            else
            {
                RemoveRange(Data);
            }
        }

        private void AddRange(IEnumerable<T> data)
        {
            _disableOnCollectionChanged = true;

            foreach (var item in data)
            {
                Items.Add(item);
            }

            _disableOnCollectionChanged = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, data));
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
        }

        private void RemoveRange(IEnumerable<T> data)
        {
            _disableOnCollectionChanged = false;
            Clear();

            _disableOnCollectionChanged = true;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, data));
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_disableOnCollectionChanged)
            {
                try
                {
                    base.OnCollectionChanged(e);
                }
                catch (NullReferenceException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
