using System;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;

namespace MAUIEssentials.Models
{
    public class BookingOptionsModel : BaseNotifyPropertyChanged
    {

    }

    public class BookingTimingModel : BaseNotifyPropertyChanged
    {
        public TimingModel? Timing { get; set; }
        public string? GroupTitle { get; set; }
        public bool IsLastItem { get; set; }

        bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BgColor));
                OnPropertyChanged(nameof(TextColor));
                OnPropertyChanged(nameof(TintColor));
            }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ItemOpacity));
            }
        }

        public double ItemOpacity => IsBusy ? 0.5 : 1;

        public Color BgColor => IsSelected
            ? AppColorResources.PrimaryGreen.ToColor()
            : AppColorResources.lightBorderColor2.ToColor();

        public Color TextColor => IsSelected
            ? AppColorResources.PrimaryGreen.ToColor()
            : AppColorResources.blackTextColor.ToColor();

        public Color TintColor => IsSelected
            ? AppColorResources.white.ToColor()
            : AppColorResources.black.ToColor();
    }

    public class BookingGroupTimingModel : BaseNotifyPropertyChanged
    {
        public List<BookingGroupTimingModel>? TimingsGroups { get; set; }
        public BookingTimingModel? BookingTimingModel { get; set; }
        public string? TitlePeriod { get; set; }
        public string? Title { get; set; }
        public string? GroupTitle { get; set; }
        public string? IsTimingAvailableMessage { get; set; }
        public bool IsSun { get; set; }
        public bool IsTimeListEmpty { get; set; }
        public bool IsTimeListView { get; set; }
        public bool IsDateTimeView { get; set; }
        public DateTime Date { get; set; }
        public int Index { get; set; }
        public string? MainTitle { get; set; }
        public List<ResourceBusyPeriodModel>? BusyOptions { get; set; }
        public bool IsResourceNotAvailabeWarningVisible => BusyOptions != null && BusyOptions.Any();

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        bool _isGroupSelected;
        public bool IsGroupSelected
        {
            get => _isGroupSelected;
            set
            {
                _isGroupSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TextColor));
            }
        }

        bool _isGroupItemSelected;
        public bool IsGroupItemSelected
        {
            get => _isGroupItemSelected;
            set
            {
                _isGroupItemSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TextColor));
            }
        }
        public Color TextColor => IsGroupItemSelected ? AppColorResources.PrimaryGreen.ToColor() : IsGroupSelected
            ? AppColorResources.blueColor.ToColor()
            : AppColorResources.darkGrayTextColor.ToColor();


        public Thickness LineMargin => Settings.AppLanguage?.Language == AppLanguage.Arabic ? new Thickness(0, 7, 0, 0) : new Thickness(0, 7, 0, 0);

        public string TimeIntervalImage => IsSun ? "ic_sun" : "ic_moon";

        public Thickness ThicknessPadding => Settings.AppLanguage?.Language == AppLanguage.English ? new Thickness(15, 0, 0, 0) : new Thickness(0, 0, 0, 15);

        public double WidthOfImage => BookingTimingModel == null ? 25 : 0;
    }
}
