using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class SubscriptionResponce
    {
        [JsonProperty(PropertyName = "links")]
        public List<ApiLinkResults>? Links { get; set; }

        [JsonProperty(PropertyName = "plans")]
        public List<SubscriptionPlan>? Plans { get; set; }

        [JsonProperty(PropertyName = "durations")]
        public List<Duration>? Durations { get; set; }
    }

    public class Duration
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "frequency")]
        public string? Frequency { get; set; }

        [JsonProperty(PropertyName = "most_popular")]
        public bool? MostPopular { get; set; }

        [JsonProperty(PropertyName = "offer_text")]
        public string? OfferText { get; set; }
    }

    public class SubscriptionPlan
    {
        [JsonProperty(PropertyName = "title")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "sub_title")]
        public string? SubTitle { get; set; }

        [JsonProperty(PropertyName = "offer_text")]
        public string? OfferText { get; set; }

        [JsonProperty(PropertyName = "features")]
        public List<string>? Features { get; set; }

        [JsonProperty(PropertyName = "subscription_group_Id")]
        public string? SubscriptionGroupId { get; set; }

        [JsonProperty(PropertyName = "products")]
        public List<SubscriptionProductsModel>? SubscriptionProductsList { get; set; }

        [JsonIgnore]
        public List<SubscriptionPriceLevel>? PriceLevels { get; set; }

        [JsonIgnore]
        public int ColumnCount => PriceLevels == null ? 0 : PriceLevels.Count;
    }
    public class SubscriptionProductsModel
    {
        [JsonProperty(PropertyName = "platform")]
        public string? Platform { get; set; }

        [JsonProperty(PropertyName = "subscription_group_Id")]
        public string? SubscriptionGroupId { get; set; }

        [JsonProperty(PropertyName = "trial_period_in_days")]
        public int? TrialPeriodInDays { get; set; }

        [JsonProperty(PropertyName = "product_ids")]
        public List<SubscriptionProductsIdModel>? SubscriptionProductsIdModelList { get; set; }
    }
    public class SubscriptionProductsIdModel
    {
        [JsonProperty(PropertyName = "product_id")]
        public string? ProductId { get; set; }

        [JsonProperty(PropertyName = "duration_id")]
        public int DurationId { get; set; }
    }

    public class FeaturesModel
    {
        public string? Feature { get; set; }
    }

    public class SubscriptionPlanLevel : BaseNotifyPropertyChanged
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Duration { get; set; }
        public string? OfferText { get; set; }
        public List<string>? Features { get; set; }
        public List<SubscriptionPriceLevel>? SubscriptionPriceLevelList { get; set; }
        public List<FeaturesModel>? FeaturesList
        {
            get
            {
                return Features?.Select(id => new FeaturesModel { Feature = id }).ToList();
            }
        }
        public bool IsFromProfile { get; set; }
        public string? CurrentProductID { get; set; }
        public bool IsBusy { get; set; }

        int? _trialPeriodInDays;
        public int? TrialPeriodInDays
        {
            get => _trialPeriodInDays;
            set
            {
                _trialPeriodInDays = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValidPriceLevel));
                OnPropertyChanged(nameof(ButtonBgColor));
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        SubscriptionPriceLevel? _selectedSubscriptionPriceLevel;
        public SubscriptionPriceLevel? SelectedSubscriptionPriceLevel
        {
            get => _selectedSubscriptionPriceLevel;
            set
            {
                _selectedSubscriptionPriceLevel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValidPriceLevel));
                OnPropertyChanged(nameof(ButtonBgColor));
                OnPropertyChanged(nameof(ButtonText));
            }
        }


        public Color? LocalizedPriceTextColor => IsBusy
                  ? AppColorResources.grayColor.ToColor() : AppColorResources.blackTextColor.ToColor();

        public bool IsValidPriceLevel => SelectedSubscriptionPriceLevel != null && string.IsNullOrEmpty(CurrentProductID);
        public Color ButtonBgColor => IsValidPriceLevel && !IsBusy
                  ? AppColorResources.blueColor.ToColor() : AppColorResources.grayColor.ToColor();
        public string ButtonText => IsBusy ? LocalizationResources.pleaseWaitString : !string.IsNullOrEmpty(CurrentProductID) ? LocalizationResources.currentPlan : IsValidPriceLevel
                 ? IsFromProfile ? LocalizationResources.switchToThisPlan :
             SelectedSubscriptionPriceLevel != null && SelectedSubscriptionPriceLevel.TrialPeriod > 0 ? string.Format(LocalizationResources.startTrialString, SelectedSubscriptionPriceLevel.TrialPeriod) :
             LocalizationResources.getStartedPayment : string.Format(LocalizationResources.noPlanForThisPeriod, Duration);
    }

    public class SubscriptionPriceLevel
    {
        public string? CurrencyCode { get; set; }
        public string? Description { get; set; }
        public string? LocalizedPrice { get; set; }
        public string? ProductId { get; set; }
        public long MicrosPrice { get; set; }
        public string? Name { get; set; }
        public int Status { get; set; }
        public Duration? Duration { get; set; }
        public string? OfferToken { get; set; }
        public string? BasePlanID { get; set; }
        public string? BillingPeriod { get; set; }
        public int TrialPeriod { get; set; }
        public List<string>? PriceList { get; set; }
        public bool IsAutoRenew { get; set; }

        public bool IsSelected { get; set; }
        public bool IsBusy { get; set; }
        public bool IsPriceVisible => (!string.IsNullOrEmpty(LocalizedPrice) && (!LocalizedPrice.ToLower().Equals("free"))) || IsBusy;

        public string PriceListString => PriceList != null && PriceList.Any() ? string.Join("\n", PriceList) : string.Empty;
        public string? FrequencyString => IsBusy ? "         " :Duration != null && MicrosPrice > 0 ? Duration.Frequency : string.Empty;

    }
}
