using System.Collections.ObjectModel;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.ViewModels;

namespace MAUIEssentials.AppCode.AlertViews
{
    public class ActionSheetViewModel : BaseViewModel
    {
        string title, cancel;
        ActionSheetConfig config;
        ObservableCollection<ActionSheetModel> actionSheetLists;

        public ActionSheetViewModel(string title, string cancel, string[] buttons, ActionSheetConfig config)
        {
            try
            {
                Title = title;
                Cancel = cancel;
                Config = config;

                var list = new List<ActionSheetModel>();

                foreach (var item in buttons)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        list.Add(new ActionSheetModel
                        {
                            Name = item,
                            FontFamily = config.ButtonsFontFamily,
                            BackgroundColor = config.ButtonsBackgroundColor,
                            BorderColor = config.BorderColor,
                            TextColor = config.ButtonsTextColor
                        });
                    }
                }

                ActionSheetSource = new ObservableCollection<ActionSheetModel>(list);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTitleVisible));
            }
        }

        public string Cancel
        {
            get => cancel;
            set
            {
                cancel = value;
                OnPropertyChanged();
            }
        }

        public ActionSheetConfig Config
        {
            get => config;
            set
            {
                config = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ActionSheetModel> ActionSheetSource
        {
            get => actionSheetLists;
            set
            {
                actionSheetLists = value;
                OnPropertyChanged();
            }
        }

        public bool IsTitleVisible => !string.IsNullOrEmpty(Title);
    }

    public class ActionSheetModel
    {
        public string? Name { get; set; }
        public string? FontFamily { get; set; }
        public Color? TextColor { get; set; }
        public Color? BackgroundColor { get; set; }
        public Color? BorderColor { get; set; }
    }
}
