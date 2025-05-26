using System;
using MAUIEssentials.AppCode.Helpers;
using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class BaseTiming
    {
        [JsonProperty(PropertyName = "from")]
        public string? From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string? To { get; set; }

        [JsonIgnore]
        public DateTime FromDateTime => DateTime.ParseExact(From ?? string.Empty, "HH:mm", CommonUtils.CurrentCulture);

        [JsonIgnore]
        public DateTime ToDateTime
        {
            get
            {
                var toDateTime = DateTime.ParseExact(To ?? string.Empty, "HH:mm", CommonUtils.CurrentCulture);
                if (toDateTime <= FromDateTime)
                {
                    toDateTime = toDateTime.AddDays(1);
                }
                return toDateTime;
            }
        }

        [JsonIgnore]
        public string FromTime => FromDateTime.ToString("hh:mm tt", CommonUtils.CurrentCulture).ConvertNumerals().ToLower();

        [JsonIgnore]
        public string ToTime => ToDateTime.ToString("hh:mm tt", CommonUtils.CurrentCulture).ConvertNumerals().ToLower();

        [JsonIgnore]
        public string TimeInterval => $"{FromTime} - {ToTime}";

        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public string TimeIntervalImage
        {
            get
            {
                if (FromDateTime.Hour > 18)
                {
                    return "ic_moon";
                }
                else if (FromDateTime.Hour > 5)
                {
                    return "ic_sun";
                }
                else
                {
                    return "ic_moon";
                }
            }
        }
    }
}