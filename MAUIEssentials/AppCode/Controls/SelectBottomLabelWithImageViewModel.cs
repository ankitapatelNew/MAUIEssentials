using System.Collections.ObjectModel;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentials.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace MAUIEssentials.AppCode.Controls
{
    public class SelectBottomLabelWithImageViewModel : BaseViewModel
    {
        public AsyncRelayCommand<BottomLabelWithImage> ItemClickCommand { get; private set; }
        public SelectBottomLabelWithImageViewModel()
        {
            ItemClickCommand = new AsyncRelayCommand<BottomLabelWithImage>(OnItemClickAction);
        }
        private async Task OnItemClickAction(BottomLabelWithImage bottomLabelWithImage)
        {
            try
            {
                var page = PageInstance as SelectBottomLabelWithImagePopup;
                await NavigationServices.ClosePopupPage();
                page.OnBottomLabelWithImageSelect?.Invoke(page, bottomLabelWithImage);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        ObservableCollection<BottomLabelWithImage> _bottomLabelWithImageList;
        public ObservableCollection<BottomLabelWithImage> BottomLabelWithImageList
        {
            get => _bottomLabelWithImageList;
            set => SetProperty(ref _bottomLabelWithImageList, value);
        }

    }

    public class BottomLabelWithImage
    {
        public BottomLabelWithImage()
        {
            Height = 25;
            ImageTintColor = AppColorResources.blueColor.ToColor();
        }
        public string Title { get; set; }
        public string Image { get; set; }
        public double Height { get; set; }
        public bool IsImageFlipInAR { get; set; }
        public Color ImageTintColor { get; set; }

        public double ImageRotation => 270;
    }
}
