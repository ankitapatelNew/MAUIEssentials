using MAUIEssentials.AppCode.Helpers;
using Mopups.Pages;

namespace MAUIEssentials.AppCode.AlertViews
{
    public partial class ActionSheetView : PopupPage
    {
        ActionSheetViewModel viewModel;
        public Action<string> Result;
        public ActionSheetView(string title, string cancel, string[] buttons, ActionSheetConfig config)
        {
            try
            {
                InitializeComponent();

                BindingContext = viewModel = new ActionSheetViewModel(title, cancel, buttons, config);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
        
        async void Handle_Clicked_Cancel(object sender, EventArgs e)
        {
            try
            {
                await NavigationServices.ClosePopupPage();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        async void Handle_Clicked(object sender, EventArgs e)
        {
            try
            {
                var text = (sender as Controls.Button).Text;
                await NavigationServices.ClosePopupPage();

                Result?.Invoke(text);
                Result = null;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}