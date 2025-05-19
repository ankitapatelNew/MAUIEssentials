namespace MAUIEssentials.AppCode.AlertViews
{
	[EditorBrowsable(EditorBrowsableState.Never)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SnackbarView : PopupPage
	{
		public SnackbarView(SnackbarConfig config)
		{
			try
            {
                InitializeComponent();
                BindingContext = new SnackbarViewModel(config, Application.Current?.Dispatcher);

                mainStack.Padding = DependencyService.Get<ICommonUtils>().IsIphoneX()
                    ? new Thickness(20, 0, 20, 35)
                    : new Thickness(20, 15);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
		}
	}
}