using System.Windows.Input;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.ViewModels;

namespace MAUIEssentials.AppCode.AlertViews
{
    public class SnackbarViewModel : BaseViewModel
    {
        private readonly IDispatcher _dispatcher;

        public SnackbarViewModel(SnackbarConfig config, IDispatcher dispatcher)
        {
            try
            {
                Config = config;
                _dispatcher = dispatcher;

                if (Config.Timeout != 0)
                {
                        _dispatcher?.StartTimer(TimeSpan.FromSeconds(Config.Timeout), () =>
                        {
                            _ = NavigationServices.ClosePopupPage();
                            return false;
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public ICommand ButtonCommand => new Command(async () => {
                try
                {
                    if (Config.ButtonCommand != null)
                    {
                        Config.ButtonCommand.Execute(null);
                    }
                    else
                    {
                        await NavigationServices.ClosePopupPage();
                    }
                }
                catch (Exception ex)
                {
                    ex.LogException();
                }
            }
        );

        SnackbarConfig _config;
        public SnackbarConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonWidth));
            }
        }

        public GridLength ButtonWidth => Config != null && Config.ButtonWidth > 0
            ? new GridLength(Config.ButtonWidth)
            : new GridLength(1, GridUnitType.Auto);
    }
}
