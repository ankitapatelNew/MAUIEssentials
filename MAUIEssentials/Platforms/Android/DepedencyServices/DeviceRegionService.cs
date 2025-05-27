using Android.Content;
using Android.OS;
using Android.Telephony;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.Platforms.Android.DepedencyServices
{
    public class DeviceRegionService : IDeviceRegionService
    {
        public string GetDeviceCountryCode()
        {
            try
            {
                var context = Platform.AppContext;
                var tm = (TelephonyManager?)context.GetSystemService(Context.TelephonyService);

                // Prefer SIM country, fallback to network country
                if (tm != null)
                {
                    // Ensure required permission if targeting Android 10+
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                    {
                        // On Android 10 and above, SimCountryIso is still accessible without runtime permissions,
                        // but NetworkCountryIso may not work without ACCESS_FINE_LOCATION (for network-based location).
                        var simCountry = tm.SimCountryIso;
                        if (!string.IsNullOrEmpty(simCountry))
                            return simCountry.ToUpper();

                        // Only fallback to network if needed
                        var networkCountry = tm.NetworkCountryIso;
                        if (!string.IsNullOrEmpty(networkCountry))
                            return networkCountry.ToUpper();
                    }
                    else
                    {
                        // Older Android versions
                        var countryCode = tm.SimCountryIso ?? tm.NetworkCountryIso;
                        return string.IsNullOrEmpty(countryCode) ? string.Empty : countryCode.ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return string.Empty;
        }
    }

}
