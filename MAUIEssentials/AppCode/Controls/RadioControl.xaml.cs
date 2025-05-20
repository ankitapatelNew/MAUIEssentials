namespace MAUIEssentials.AppCode.Controls;

public partial class RadioControl : StackLayout
{
	public event EventHandler<RadioSelectedEventArgs> ValueSelected;

	public static readonly BindableProperty ItemSourceProperty =
		BindableProperty.Create(nameof(ItemSource), typeof(IEnumerable<RadioModel>), typeof(RadioControl), null);

	public static readonly BindableProperty TextColorProperty =
		BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(RadioControl), Colors.Black);

	public static readonly BindableProperty FontSizeProperty =
		BindableProperty.Create(nameof(FontSize), typeof(double), typeof(RadioControl), 16d);

	public Color TextColor
	{
		get => (Color)GetValue(TextColorProperty);
		set => SetValue(TextColorProperty, value);
	}

	public double FontSize
	{
		get => (double)GetValue(FontSizeProperty);
		set => SetValue(FontSizeProperty, value);
	}

	public IEnumerable<RadioModel> ItemSource
	{
		get => (IEnumerable<RadioModel>)GetValue(ItemSourceProperty);
		set => SetValue(ItemSourceProperty, value);
	}

	public RadioControl()
	{
		InitializeComponent();
	}

	void Border_Clicked(object sender, EventArgs e)
	{
		try
		{
			var name = string.Empty;
			var list = ItemSource.ToList();

			if (sender is Border border && border.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tapGesture)
			{
				var selectedValue = tapGesture.CommandParameter;
				var selectedItem = list.FirstOrDefault(x => x.Name == selectedValue.ToString());

				if (selectedItem.IsCheckbox)
				{
					selectedItem.IsSelected = !selectedItem.IsSelected;
					var selectedList = list.Where(x => x.IsSelected).ToList();

					for (int i = 0; i < selectedList.Count; i++)
					{
						if (i == selectedList.Count - 1)
						{
							name += selectedList[i].Name;
						}
						else
						{
							name += selectedList[i].Name + ",";
						}
					}
				}
				else
				{
					name = selectedValue.ToString();

					foreach (var item in list)
					{
						item.IsSelected = item.Name == selectedValue.ToString();
					}
				}	
			}

			ValueSelected?.Invoke(this, new RadioSelectedEventArgs { Name = name });
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}
}

public class RadioSelectedEventArgs : EventArgs
{
    public string? Name { get; set; }
}

public class RadioModel : BaseNotifyPropertyChanged
{
    public string? Name { get; set; }
    public bool IsCheckbox { get; set; }

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

    public string RadioImage => IsCheckbox
        ? CommonUtils.SetCheckImage(IsSelected)
        : CommonUtils.SetRadioImage(IsSelected);

    public Color RadioTintColor => CommonUtils.SetTintColor(IsSelected);
}