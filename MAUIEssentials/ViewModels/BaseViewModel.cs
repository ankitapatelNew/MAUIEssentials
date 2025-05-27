
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Models;
using Newtonsoft.Json;

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

        readonly WeakEventManager propertyChangedEventManager = new WeakEventManager();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => propertyChangedEventManager.AddEventHandler(value);
            remove => propertyChangedEventManager.RemoveEventHandler(value);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = propertyChangedEventManager;

            if (changed == null)
            {
                return;
            }

            changed.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(PropertyChanged));
        }

        public async void OnException(Exception ex)
        {
            await CommonUtils.HideLoader();
            ex.LogException();
        }

        public virtual void OnFirstTimeAppearing() { }

        public virtual void OnAppearing() { }

        public virtual void OnDisappearing()
        {
            IsRefreshing = false;
        }

        public virtual void OnNotificationReceived(IDictionary<string, object> data) { }

        public virtual void OnInternetAvailable() { }

        public virtual void OnDispose() { }

        public void CheckInternetConnected()
        {
            OnPropertyChanged(nameof(IsInternetConnected));
        }

        public bool IsInternetConnected => Connectivity.NetworkAccess == NetworkAccess.Internet
            || Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet;

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

        public string RigthArrow => Settings.AppLanguage?.Language == AppLanguage.Arabic ? "ic_arrow_left" : "ic_arrow_right";
        public string BackImage =>  Settings.AppLanguage?.Language == AppLanguage.Arabic ? "ic_back_right" : "ic_back";

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
        
        public double ItemSize
        {
            get
            {
                var width = (ScreenSize.ScreenWidth - 40) / ((DeviceInfo.Idiom == DeviceIdiom.Tablet) ? 4 :2.9);
                return width - 30;
            }
        }

        public double MainItemHeight
        {
            get
            {
                return (ScreenSize.ScreenWidth - 40) / ((DeviceInfo.Idiom == DeviceIdiom.Tablet) ? 4.5 :3) + 10;
            }
        }

        public double MainItemWidth
        {
            get
            {
                return (ScreenSize.ScreenWidth - 40) / ((DeviceInfo.Idiom == DeviceIdiom.Tablet) ? 3 :2);
            }
        }

        public double ItemCornerRadius => ItemSize / 2;
        public double MainItemVerticalSpacing => (DeviceInfo.Idiom == DeviceIdiom.Tablet) ? 30 : 15;
    }
}
