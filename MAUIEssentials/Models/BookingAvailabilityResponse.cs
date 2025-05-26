using System;
using System.Globalization;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class BookingAvailabilityResponse : ApiLinks
    {
        [JsonProperty(PropertyName = "availabile_days_timings")]
        public List<AvailabileDaysTiming>? AvailabileDaysTimings { get; set; }

        [JsonProperty(PropertyName = "result")]
        public AvailabileDaysTiming? AvailabileDaysTiming { get; set; }

        [JsonProperty(PropertyName = "card_payment_enabled")]
        public bool cardPaymentEnabled { get; set; }

        [JsonProperty(PropertyName = "cash_payment_enabled")]
        public bool cashPaymentEnabled { get; set; }

        [JsonProperty(PropertyName = "show_device_timezone")]
        public bool showDeviceTimezone { get; set; }

    }

    public class AvailabileDaysTiming
    {

        [JsonProperty(PropertyName = "busy_options")]
        public List<ResourceBusyPeriodModel>? BusyOptions { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string? DateStr { get; set; }

        //[JsonProperty(PropertyName = "timings")]
        //public List<TimingModel> Timings { get; set; }

        [JsonProperty(PropertyName = "timings_groups")]
        public List<TimingGroupModel>? TimingsGroups { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string? Message { get; set; }

        public DateTime Date => DateTime.ParseExact(DateStr ?? string.Empty, "yyyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US"));

        public string ShortMonthName => CommonUtils.GetMonthName(Date);

        public string LocalDateString => string.Format("{0} {1} {2}", Date.Day.ToString().ConvertNumerals(), ShortMonthName, Date.Year.ToString().ConvertNumerals());

        public string LocalDay => Settings.AppLanguage?.Language == AppLanguage.Arabic ? Date.Day.ToString().ConvertNumerals() : Date.Day.ToString();

        [JsonIgnore]
        public List<BookingGroupTimingModel>? BookingGroupTimingModelList { get; set; }
    }

    public class TimingGroupModel
    {
        [JsonProperty(PropertyName = "title")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "title_period")]
        public string? TitlePeriod { get; set; }

        [JsonProperty(PropertyName = "is_sun")]
        public bool IsSun { get; set; }

        [JsonProperty(PropertyName = "timings")]
        public List<TimingModel>? Timings { get; set; }

        [JsonIgnore]
        public string TimeIntervalImage => IsSun ? "ic_sun" : "ic_moon";
    }

    public class TimingModel : BaseTiming
    {
        [JsonProperty(PropertyName = "selection")]
        public string? Selection { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int DurationInMinutes { get; set; }
    }



    public class ResourceBusyPeriodModel
    {
        [JsonProperty(PropertyName = "option_name")]
        public string? ResourceName { get; set; }

        [JsonProperty(PropertyName = "unavailable_string")]
        public string? UnavailableString { get; set; }

        [JsonIgnore]
        public string ResourcePeriodString => string.Format(LocalizationResources.resourceBusyPeriod, ResourceName, UnavailableString);
    }

    public class AvailableOptionResponse
    {
        [JsonProperty("available_options")]
        public List<AvailableOption>? AvailableOptions { get; set; }
    }

    public class AvailableOption : ApiLinks
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("entity_location_option_id")]
        public int EntityLocationOptionId { get; set; }

        [JsonProperty("image_url")]
        public string? ImageUrl { get; set; }
    }
}
