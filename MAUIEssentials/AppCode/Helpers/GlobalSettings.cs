namespace MAUIEssentials.AppCode.Helpers
{
    public static class GlobalSettings
    {
        public static DialogSettings DialogSettings = new DialogSettings();
    }

    public class DialogSettings : INotifyPropertyChanged
    {
        bool _isOverlay;

        public bool IsOverlay
        {
            get
            {
                return _isOverlay;
            }
            set
            {
                _isOverlay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOverlay)));
            }
        }

        public void ToggleOverlay(bool status)
        {
            IsOverlay = status;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
