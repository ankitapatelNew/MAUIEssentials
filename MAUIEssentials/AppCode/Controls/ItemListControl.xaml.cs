using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class ItemListControl : StackLayout
	{
		public event EventHandler OnDeleted;

		public static readonly BindableProperty HeaderTextProperty =
			BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(ItemListControl), string.Empty);

		public static readonly BindableProperty SourceProperty =
			BindableProperty.Create(nameof(Source), typeof(IEnumerable<ItemListModel>), typeof(ItemListControl), null);

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create(nameof(SelectedItem), typeof(ItemListModel), typeof(ItemListControl), null);

		public string HeaderText
		{
			get => (string)GetValue(HeaderTextProperty);
			set => SetValue(HeaderTextProperty, value);
		}

		public IEnumerable<ItemListModel> Source
		{
			get => (IEnumerable<ItemListModel>)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		public ItemListModel SelectedItem
		{
			get => (ItemListModel)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public ItemListControl()
		{
			InitializeComponent();
		}

		void DeleteTapped(object sender, EventArgs e)
		{
			try
			{
				SelectedItem = Source.FirstOrDefault(x => x.Description == (sender as Grid).ClassId) ?? new ItemListModel();
				OnDeleted?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}

public class ItemListModel
{
    public string? Icon { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Color? IconTintColor { get; set; }
}