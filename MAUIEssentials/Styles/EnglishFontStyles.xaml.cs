namespace MAUIEssentials.Styles;

public partial class EnglishFontStyles : ResourceDictionary
{
	public EnglishFontStyles()
	{
		InitializeComponent();
	}

	public static EnglishFontStyles Instance { get; } = new EnglishFontStyles();
}