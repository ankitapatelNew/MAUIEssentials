namespace MAUIEssentials.AppCode.DependencyServices
{
    public interface IKeyboardService
    {
        event EventHandler KeyboardIsShown;
        event EventHandler KeyboardIsHidden;
        void HideKeyboard();
        double KeyboardHeight();
    }
}