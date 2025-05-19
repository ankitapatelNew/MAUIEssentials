
namespace MAUIEssentials.ViewModels
{
    public class BaseViewModel : IQueryAttributable, INotifyPropertyChanged
    {
        public Page? PageInstance { get; set; }

        bool isBusy = false;

        [JsonIgnore]
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

         public virtual void OnFirstTimeAppearing() { }

        public virtual void OnAppearing() { }

        public virtual void OnDisappearing() { }

        public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
        {

        }

        protected T SetPropertyFromQuery<T>(IDictionary<string, object> query, string key, T property, T defaultValue = default)
        {
            if (query.TryGetValue(key, out var value) && value is T typedValue)
            {
                property = typedValue;
            }
            else
            {
                property = defaultValue;
            }
            return property;
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        string? _apiErrorMessage;
        public string? ApiErrorMessage
        {
            get => _apiErrorMessage;
            set => SetProperty(ref _apiErrorMessage, value);
        }
    }
}
