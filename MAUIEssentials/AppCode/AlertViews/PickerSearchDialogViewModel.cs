namespace MAUIEssentials.AppCode.AlertViews
{
    public class PickerSearchDialogViewModel : BaseViewModel
    {
        string _searchText;
        ObservableCollection<PickerModel> _filterDataList;
        ObservableCollection<PickerModel> _dataList;
        PickerDialogSettings _settings;

        public PickerSearchDialogViewModel(PickerDialogSettings settings)
        {
            try
            {
                PickerDialog = settings;

                var filterList = PickerDialog.ListData
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .Select(x => new PickerModel { Name = x, IsSelected = x == PickerDialog.SelectedObject })
                    .ToList();

                FilterDataList = new ObservableCollection<PickerModel>(filterList);
                DataList = new ObservableCollection<PickerModel>(filterList);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
        void SearchData()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    DataList = new ObservableCollection<PickerModel>(FilterDataList);
                }
                else
                {
                    var searchedData = FilterDataList
                        .Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    DataList = new ObservableCollection<PickerModel>(searchedData);
                }

                OnPropertyChanged(nameof(DataList));
                OnPropertyChanged(nameof(ListHeight));
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public ObservableCollection<PickerModel> DataList
        {
            get => _dataList;
            set
            {
                _dataList = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListHeight));
            }
        }

        public ObservableCollection<PickerModel> FilterDataList
        {
            get => _filterDataList;
            set
            {
                _filterDataList = value;
                OnPropertyChanged();
            }
        }

        public PickerDialogSettings PickerDialog
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value)
                    return;

                _searchText = value;
                SearchData();
                OnPropertyChanged(); 
            }
        }

        public double ListHeight
        {
            get
            {
                if (DataList == null || !DataList.Any())
                {
                    return 160;
                }

                return DataList.Count > 5 ? 160 : DataList.Count * 40;
            }
        }
    }

    public class PickerModel : BaseNotifyPropertyChanged
    {
        public string Name { get; set; }

        bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RadioImage));
                OnPropertyChanged(nameof(RadioTintColor));
            }
        }

        public bool IsHtml => Name.IsHtml();

        public string RadioImage => CommonUtils.SetRadioImage(IsSelected);
        public Color RadioTintColor => CommonUtils.SetTintColor(IsSelected);
    }

    public class PickerDialogSettings : BaseViewModel
    {
        bool _isSearchVisible = true;
        string _okText = "OK";
        string _cancelText = "Cancel";
        string _titleText = "Select";
        string _searchPlaceholder = "Search";
        Color _cancelTextColor = Colors.Black;
        Color _okTextColor = Colors.Black;
        Color _cancelBackgroundColor = Colors.LightGray;
        Color _okBackgroundColor = Colors.LightGray;
        Color _titleTextColor = Colors.Black;
        Color _tintColor = Colors.Transparent;

        public Color CancelTextColor
        {
            get => _cancelTextColor;
            set
            {
                _cancelTextColor = value;
                OnPropertyChanged();
            }
        }

        public Color OkTextColor
        {
            get => _okTextColor;
            set
            {
                _okTextColor = value;
                OnPropertyChanged();
            }
        }

        public Color CancelBackgroundColor
        {
            get => _cancelBackgroundColor;
            set
            {
                _cancelBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public Color OkBackgroundColor
        {
            get => _okBackgroundColor;
            set
            {
                _okBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public Color TitleTextColor
        {
            get => _titleTextColor;
            set
            {
                _titleTextColor = value;
                OnPropertyChanged();
            }
        }

        public string OkText
        {
            get => _okText;
            set
            {
                _okText = value;
                OnPropertyChanged();
            }
        }

        public string CancelText
        {
            get => _cancelText;
            set
            {
                _cancelText = value;
                OnPropertyChanged();
            }
        }

        public string TitleText
        {
            get => _titleText;
            set
            {
                _titleText = value;
                OnPropertyChanged();
            }
        }

        public bool IsSearchVisible
        {
            get => _isSearchVisible;
            set
            {
                _isSearchVisible = value;
                OnPropertyChanged();
            }
        }

        public Color TintColor
        {
            get => _tintColor;
            set
            {
                _tintColor = value;
                OnPropertyChanged();
            }
        }

        public string SearchPlaceholder
        {
            get => _searchPlaceholder;
            set
            {
                _searchPlaceholder = value;
                OnPropertyChanged();
            }
        }

        public string SelectedObject { get; set; }
        public string SearchIcon { get; set; }
        public string CloseIcon { get; set; }
        public List<string> ListData { get; set; }
        public Action<string> Search { get; set; }
    }
}
