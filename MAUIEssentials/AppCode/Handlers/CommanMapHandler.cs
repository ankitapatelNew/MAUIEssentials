using MAUIEssentials.AppCode.Controls;
using Microsoft.Maui.Maps.Handlers;

namespace MAUIEssentials
{
    public partial class CommanMapHandler
    {
        public static IPropertyMapper<CustomMap, IMapHandler> PropertyMapper = new PropertyMapper<CustomMap, IMapHandler>(ViewMapper)
        {
            [nameof(CustomMap.MapType)] = UpdateMapType,
            [nameof(CustomMap.IsShowingUser)] = UpdateIsShowingUser,
            [nameof(CustomMap.IsScrollEnabled)] = UpdateHasScrollEnabled,
            [nameof(CustomMap.IsZoomEnabled)] = UpdateHasZoomEnabled,
            [nameof(CustomMap.IsTrafficEnabled)] = UpdateTrafficEnabled
        };

        public CommanMapHandler() : base(PropertyMapper)
        {
        }
    }
}
