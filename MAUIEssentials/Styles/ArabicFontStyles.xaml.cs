namespace MAUIEssentials.Styles;

public partial class ArabicFontStyles : ResourceDictionary
{
	public ArabicFontStyles()
	{
		InitializeComponent();
	}

	public static ArabicFontStyles Instance { get; } = new ArabicFontStyles();
}