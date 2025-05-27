using Foundation;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.Platforms.iOS.DependencyServices
{
    public class DeviceRegionService: IDeviceRegionService
    {
        public string GetDeviceCountryCode()
        {
            try
            {
                var countryCode = NSLocale.CurrentLocale.CountryCode;
                return string.IsNullOrEmpty(countryCode) ? string.Empty : countryCode.ToUpper();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            
            return string.Empty;
        }
    }
}