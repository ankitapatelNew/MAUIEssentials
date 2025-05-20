namespace MAUIEssentials.Styles
{
	public partial class SmallDevicesStyle : ResourceDictionary
	{
		public SmallDevicesStyle()
		{
			InitializeComponent();
		}
		public static SmallDevicesStyle Instance { get; } = new SmallDevicesStyle();
	}
}