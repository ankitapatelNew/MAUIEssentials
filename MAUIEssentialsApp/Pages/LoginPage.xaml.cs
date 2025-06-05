using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Pages;

namespace MAUIEssentialsApp.Pages;

public partial class LoginPage : BaseContentPage
{
	public LoginPage()
	{
		try
		{
			InitializeComponent();
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

    protected override bool OnBackButtonPressed()
    {
        return base.OnBackButtonPressed();
    }
}