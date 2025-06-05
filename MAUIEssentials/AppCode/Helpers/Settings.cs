using System;
using MAUIEssentials.Models;
using Newtonsoft.Json;

namespace MAUIEssentials.AppCode.Helpers
{
    public static class Settings
    {
        const string AppLanguageKey = "AppLanguageKey";
        static readonly string AppLanguageDefault = string.Empty;

        const string HuaweiAppKey = "HuaweiAppKey";
        static readonly bool HuaweiAppDefault = false;

        const string AppTypeKey = "AppTypeKey";
        static readonly string AppTypeDefault = string.Empty;

        const string OwnerSubscriptionModelKey = "OwnerSubscriptionModelKey";
        static readonly string OwnerSubscriptionModelDefault = string.Empty;

        const string SubscriptionResponseKey = "SubscriptionResponseKey";
        static readonly string SubscriptionResponseDefault = string.Empty;

        const string FingerprintEnabledKey = "FingerprintEnabledKey";
        static readonly bool FingerprintEnabledDefault = false;

        static LanguageModel? _appLanguage;
        public static LanguageModel? AppLanguage
        {
            get
            {
                if (_appLanguage != null)
                    return _appLanguage;

                var appLanguage = Preferences.Get(AppLanguageKey, AppLanguageDefault);
                _appLanguage = appLanguage != null ? JsonConvert.DeserializeObject<LanguageModel>(appLanguage) : null;
                return _appLanguage;
            }
            set
            {
                _appLanguage = value;
                Preferences.Set(AppLanguageKey, JsonConvert.SerializeObject(value));
            }
        }

        public static bool IsHuaweiApp
        {
            get => Preferences.Get(HuaweiAppKey, HuaweiAppDefault);
            set => Preferences.Set(HuaweiAppKey, value);
        }

        public static AppTypeEnum AppType
        {
            get => Preferences.Get(AppTypeKey, AppTypeDefault).ToEnum(AppTypeEnum.Customer);
            set => Preferences.Set(AppTypeKey, value.ToString());
        }

        static List<SubscriptionPlanLevel>? _subscriptionPlanLevelList;
        public static List<SubscriptionPlanLevel>? SubscriptionPlanLevelList
        {
            get
            {
                if (_subscriptionPlanLevelList != null)
                    return _subscriptionPlanLevelList;
                var data = Preferences.Get(OwnerSubscriptionModelKey, OwnerSubscriptionModelDefault);
                _subscriptionPlanLevelList = string.IsNullOrEmpty(data) ? new List<SubscriptionPlanLevel>() : JsonConvert.DeserializeObject<List<SubscriptionPlanLevel>>(data);
                return _subscriptionPlanLevelList;
            }
            set
            {
                _subscriptionPlanLevelList = value;
                Preferences.Set(OwnerSubscriptionModelKey, value == null ? string.Empty : JsonConvert.SerializeObject(value));
            }
        }


        static SubscriptionResponce? _subscriptionResponse;
        public static SubscriptionResponce? SubscriptionResponse
        {
            get
            {
                if (_subscriptionResponse != null)
                    return _subscriptionResponse;

                var data = Preferences.Get(SubscriptionResponseKey, SubscriptionResponseDefault);
                _subscriptionResponse = string.IsNullOrWhiteSpace(data)
                    ? null
                    : JsonConvert.DeserializeObject<SubscriptionResponce>(data);

                return _subscriptionResponse;
            }
            set
            {
                _subscriptionResponse = value;
                Preferences.Set(SubscriptionResponseKey, value == null ? string.Empty : JsonConvert.SerializeObject(value));
            }
        }

        public static bool FingerprintEnabled
        {
            get => Preferences.Get(FingerprintEnabledKey, FingerprintEnabledDefault);
            set => Preferences.Set(FingerprintEnabledKey, value);
        }
    }
}
