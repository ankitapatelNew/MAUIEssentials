using MAUIEssentials.AppCode.Controls;

namespace MAUIEssentials.Handlers
{
    public partial class CommanPickerViewHandler
    {
        public static IPropertyMapper<PickerView, CommanPickerViewHandler> PropertyMapper = new PropertyMapper<PickerView, CommanPickerViewHandler>(ViewMapper)
        {
            [nameof(PickerView.ItemsSource)] = UpdateItemsSource,
            [nameof(PickerView.FontSize)] = UpdateFont,
            [nameof(PickerView.FontFamily)] = UpdateFont,
            [nameof(PickerView.SelectedItemFontFamily)] = UpdateFont,
            [nameof(PickerView.SelectedIndex)] = UpdateSelectedIndex
        };

        public CommanPickerViewHandler() : base(PropertyMapper)
        {
        }
    }
}
