namespace MAUIEssentials.Styles
{
	public partial class DefaultDevicesStyle : ResourceDictionary
	{
		public DefaultDevicesStyle()
		{
			InitializeComponent();
		}
		public static DefaultDevicesStyle Instance { get; } = new DefaultDevicesStyle();
	}
}