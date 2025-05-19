namespace MAUIEssentials.AppCode.AlertViews
{
	public partial class AlertView : PopupPage
	{
		public Action<bool> Result;
		public AlertView(string title, string message, AlertConfig config, string positiveText = "", string negativeText = "")
		{
			try
			{
				InitializeComponent();

				lblTitle.Text = title;
				lblMessage.IsVisible = !message.IsHtml();
				lblHtml.IsVisible = message.IsHtml();

				if (message.IsHtml())
				{
					lblHtml.Text = message;
					lblHtml.LinkColor = config.PositiveColor;
				}
				else
				{
					lblMessage.Text = message;
				}

				if (string.IsNullOrEmpty(title))
				{
					lblTitle.IsVisible = false;
					lblMessage.Margin = new Thickness(10, 15);
				}

				bool canShowTwoButton = !string.IsNullOrEmpty(negativeText);
				twoButtonView.IsVisible = canShowTwoButton;
				singleButtonView.IsVisible = !canShowTwoButton;

				if (!string.IsNullOrEmpty(positiveText))
				{
					btnPositive.Text = positiveText;
					singleButtonView.Text = positiveText;
				}

				if (!string.IsNullOrEmpty(negativeText))
				{
					btnNegative.Text = negativeText;
				}

				ApplyConfig(config);

				if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
				{
					border.WidthRequest = 350;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
		
		public void ApplyConfig(AlertConfig config)
		{
			border.Margin = config.Margin;

			border.BackgroundColor = config.BackgroundColor;
			lblTitle.TextColor = config.TitleColor;
			lblMessage.TextColor = config.MessageColor;
			btnPositive.TextColor = config.PositiveColor;
			singleButtonView.TextColor = config.PositiveColor;
			btnPositive.BackgroundColor = config.PositiveButtonColor;
			btnNegative.TextColor = config.NegativeColor;
			btnNegative.BackgroundColor = config.NegativeButtonColor;
			separator1.BackgroundColor = config.SeparatorColor;
			separator2.BackgroundColor = config.SeparatorColor;

			lblTitle.FontFamily = config.TitleFontFamily;
			lblMessage.FontFamily = config.MessageFontFamily;
			btnPositive.FontFamily = config.PositiveButtonFontFamily;
			btnNegative.FontFamily = config.NegativeButtonFontFamily;
			singleButtonView.FontFamily = config.PositiveButtonFontFamily;
		}

		async void Handle_Clicked_PositiveButton(object sender, EventArgs e)
		{
			await SetResult(true);
		}

		async void Handle_Clicked_NegativeButton(object sender, EventArgs e)
		{
			await SetResult(false);
		}

		async void Handle_Clicked_SingleButton(object sender, EventArgs e)
		{
			await SetResult(true);
		}

		private async Task SetResult(bool status)
		{
			await Navigation.PopAsync();
			Result?.Invoke(status);
			Result = null;
		}
	}
}