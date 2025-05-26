using CommunityToolkit.Maui.Behaviors;
using MAUIEssentials.AppCode.Helpers;
using Mopups.Pages;
using Mopups.Services;

namespace MAUIEssentials.AppCode.AlertViews
{
	public partial class AlertView : PopupPage
	{
		public Action<bool> Result;
		public AlertView(string title, string message, AlertConfig config, string positiveText = "", string negativeText = "", string SecondMessage = "", string imageError = "", string tintColor = "")
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

				if (config.MessageFormattedString != null)
				{
					lblHtml.IsVisible = false;
					lblMessage.Text = string.Empty;

					lblMessage.FormattedText = config.MessageFormattedString;
				}

				if (string.IsNullOrEmpty(title))
				{
					lblTitle.IsVisible = false;
					lblMessage.Margin = new Thickness(10, 15);
				}

				if (!string.IsNullOrEmpty(imageError))
				{
					image.Source = imageError;
					image.IsVisible = true;
				}
				else
				{
					image.IsVisible = false;
				}

				if (!string.IsNullOrEmpty(SecondMessage))
				{
					lblSecondMessage.Text = SecondMessage;
					lblSecondMessage.IsVisible = true;
				}
				else
				{
					lblSecondMessage.IsVisible = false;
				}

				if (!string.IsNullOrEmpty(tintColor))
				{
					var behavior = new IconTintColorBehavior
					{
						TintColor = tintColor.ToColor()
					};
					image.Behaviors.Add(behavior);
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
				else
				{
					btnNegative.IsVisible = false;
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
			try
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

				lblTitle.FontFamily = config.TitleFontFamily;
				lblMessage.FontFamily = config.MessageFontFamily;
				btnPositive.FontFamily = config.PositiveButtonFontFamily;
				btnNegative.FontFamily = config.NegativeButtonFontFamily;
				singleButtonView.FontFamily = config.PositiveButtonFontFamily;
				
				if (config.PositiveBorderColor != null && config.PositiveBorderColor != new Color())
				{
					btnPositive.BorderWidth = 1;
					btnPositive.BorderColor = config.PositiveBorderColor;
				}

				if (config.NegativeBorderColor != null && config.NegativeBorderColor != new Color())
				{
					btnNegative.BorderWidth = 1;
					btnNegative.BorderColor = config.NegativeBorderColor;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
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

		protected override bool OnBackButtonPressed()
        { 
            _= SetResult(false);
            return true;
        }

		private async Task SetResult(bool status)
		{
			try
			{
				await MopupService.Instance.PopAsync();
				Result?.Invoke(status);
				Result = null;
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}