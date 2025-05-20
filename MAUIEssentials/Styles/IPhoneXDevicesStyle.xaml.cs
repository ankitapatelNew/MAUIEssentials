namespace MAUIEssentials.Styles
{
	public partial class IPhoneXDevicesStyle : ResourceDictionary
	{
		public IPhoneXDevicesStyle()
		{
			InitializeComponent();
		}
		public static IPhoneXDevicesStyle Instance { get; } = new IPhoneXDevicesStyle();
	}
}