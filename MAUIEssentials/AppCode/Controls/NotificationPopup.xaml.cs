namespace MAUIEssentials.AppCode.Controls
{
	public partial class NotificationPopup : PopupPage
	{
		public Action OkTapped { get; set; }
		public Action SecondButtonTapped { get; set; }

		public NotificationPopup(NotificationType type, string title, string msg, bool isHtml = false, string secondBtnText = "")
		{
			try
			{
				InitializeComponent();

				lblTitle.Text = title;

				btnTwo.IsVisible = !string.IsNullOrEmpty(secondBtnText);
				btnTwo.Text = secondBtnText;

				lblHtmlMsg.IsVisible = isHtml;
				lblMsg.IsVisible = !isHtml;

				if (isHtml)
				{
					lblHtmlMsg.Text = msg;
				}
				else
				{
					lblMsg.Text = msg;
				}

				switch (type)
				{
					case NotificationType.Error:
						imgType.Source = "ic_type_error";
						break;
					case NotificationType.Failed:
						imgType.Source = "ic_type_failed";
						break;
					case NotificationType.Info:
						imgType.Source = "ic_type_info";
						break;
					case NotificationType.Success:
						imgType.Source = "ic_type_success";
						break;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
		
		async void CloseTapped(object sender, EventArgs e)
		{
			try
			{
				await NavigationServices.ClosePopupPage();

				if (sender is Button)
				{
					OkTapped?.Invoke();
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void SecondButtonClicked(object sender, EventArgs e)
		{
			try
			{
				await NavigationServices.ClosePopupPage();
				SecondButtonTapped?.Invoke();
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void HtmlOverrideLinkClicked(object sender, EventArgs e)
		{
			try
			{

			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}