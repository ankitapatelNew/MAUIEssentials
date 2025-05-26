using System.Collections.ObjectModel;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class SelectBottomLabelWithImagePopup : PopupBase
	{
		public Action<SelectBottomLabelWithImagePopup, BottomLabelWithImage> OnBottomLabelWithImageSelect { get; set; }
		public SelectBottomLabelWithImagePopup(List<BottomLabelWithImage> bottomLabelWithImageList)
		{
			try
			{
				InitializeComponent();
				viewModel.PageInstance = this;
				viewModel.BottomLabelWithImageList = new ObservableCollection<BottomLabelWithImage>(bottomLabelWithImageList);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
		protected override bool OnBackButtonPressed()
		{
			OnBottomLabelWithImageSelect?.Invoke(this, null);
			return base.OnBackButtonPressed();
		}

		protected override bool OnBackgroundClicked()
		{
			OnBottomLabelWithImageSelect?.Invoke(this, null);
			return base.OnBackgroundClicked();
		}
	}
}