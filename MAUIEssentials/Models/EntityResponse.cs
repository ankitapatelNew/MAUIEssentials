using System;
using System.Collections.ObjectModel;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class EntityResponse : ApiLinks
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "available_filters")]
        public List<Filter>? Filters { get; set; }

        [JsonProperty(PropertyName = "entities")]
        public List<Entity>? Entities { get; set; }
    }

    public class CommonEntity : ApiLinks
    {
        [JsonProperty(PropertyName = "entity_location")]
        public int EntityLocation { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string? Address { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public string? Rating { get; set; }

        [JsonProperty(PropertyName = "rating_value")]
        public double? RatingValue { get; set; }

        [JsonProperty(PropertyName = "hygiene_rating")]
        public string? HygieneRating { get; set; }

        [JsonProperty(PropertyName = "popular_measure")]
        public int? PopularMeasure { get; set; }

        [JsonProperty(PropertyName = "location")]
        public EntityLocation? Location { get; set; }

        [JsonProperty(PropertyName = "opening_hours")]
        public List<OpeningHours>? OpeningHours { get; set; }

        [JsonProperty(PropertyName = "category_id")]
        public int CategoryId { get; set; }

        [JsonProperty(PropertyName = "category_name")]
        public string? CategoryName { get; set; }

        public bool IsNoRating => string.IsNullOrEmpty(Rating);

        public bool IsBusy { get; set; }

        [JsonProperty(PropertyName = "offer_text")]
        public string? OfferText { get; set; }

        [JsonProperty(PropertyName = "rating_total_count")]
        public string? RatingTotalCount { get; set; }

        [JsonProperty(PropertyName = "average_cost_level")]
        public string? AverageCostLevel { get; set; }

        [JsonProperty(PropertyName = "has_subscriptions")]
        public bool HasSubscriptions { get; set; }

        [JsonProperty(PropertyName = "reviews_visible")]
        public bool ReviewsVisible { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public ObservableCollection<string>? Tags { get; set; }

        [JsonProperty(PropertyName = "services_title")]
        public string? ServicesTitle { get; set; }

        [JsonProperty(PropertyName = "resources_title")]
        public string? ResourcesTitle { get; set; }

        [JsonProperty(PropertyName = "subscriptions_title")]
        public string? SubscriptionsTitle { get; set; }

        [JsonProperty(PropertyName = "has_services")]
        public bool HasServices { get; set; }

        [JsonProperty(PropertyName = "has_resources")]
        public bool HasResources { get; set; }

        [JsonProperty(PropertyName = "departments_title")]
        public string? DepartmentsTitle { get; set; }

        [JsonProperty(PropertyName = "has_departments")]
        public bool HasDepartments { get; set; }

        [JsonProperty(PropertyName = "services_visible")]
        public bool ServicesVisible { get; set; }

        [JsonProperty(PropertyName = "online_booking_enabled")]
        public bool OnlineBookingEnabled { get; set; }
    }

    public class Entity : CommonEntity
    {
        [JsonProperty(PropertyName = "filters")]
        public List<int>? Filters { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string? Summary { get; set; }

        [JsonProperty(PropertyName = "distance")]
        public string? Distance { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string? Image { get; set; }

        [JsonProperty(PropertyName = "hygiene_rating_description")]
        public string? HygieneDescription { get; set; }

        [JsonProperty(PropertyName = "offer_text")]
        public string? OfferText { get; set; }

        public double DistanceFromUser { get; set; } = -1;

        [JsonIgnore]
        public ImageSource ImageSource
        {
            get
            {
                if (!string.IsNullOrEmpty(Image) && Image.StartsWith("http"))
                {
                    return ImageSource.FromUri(new Uri(Image));
                }
                else
                {
                    return ImageSource.FromFile(!string.IsNullOrEmpty(Image) ? Image : "ic_placeholder");
                }
            }
        }

        public string DistanceFromUserValue
        {
            get
            {
                if (DistanceFromUser * 1000 < 0)
                {
                    return string.Empty;
                }

                if (DistanceFromUser > 1)
                {
                    return $"{string.Format("{0:#,###.0}", DistanceFromUser)} {LocalizationResources.km}";
                }

                return DistanceFromUser * 1000 < 1
                    ? $"{string.Format("{0:#.0}", (DistanceFromUser * 1000).ToString("0"))} {LocalizationResources.meter}"
                    : $"{string.Format("{0:#,###.0}", DistanceFromUser * 1000)} {LocalizationResources.meter}";
            }
        }

        public string RateText => string.IsNullOrEmpty(Rating) ? LocalizationResources.notRatedYet : Rating.ConvertNumerals();
        public Color RateTintColor => string.IsNullOrEmpty(Rating) ? AppColorResources.Gray30Opacity.ToColor() : AppColorResources.Yellow.ToColor();
        public Color RateTextColor => string.IsNullOrEmpty(Rating) ? AppColorResources.Gray30Opacity.ToColor() : AppColorResources.blackTextColor.ToColor();
        public bool IsAdsShown { get; set; }
    }
    public class EntityDepartmentsResponse : ApiLinks
    {
        [JsonProperty("result")]
        public List<EntityDepartments>? EntityDepartments { get; set; }
    }
    public class EntityDepartments
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string? ImageUrl { get; set; }

        [JsonProperty(PropertyName = "sub_departments")]
        public List<EntityDepartments>? SubDepartments { get; set; }

        public bool IsBusy { get; set; }
    }

    public class EntityLocation
    {
        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }
    }

    public class OpeningHours
    {
        [JsonProperty(PropertyName = "day")]
        public Day? Day { get; set; }
    }

    public class Day
    {
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        public string ShortName => !string.IsNullOrEmpty(Name)
            ? Settings.AppLanguage?.Language == AppLanguage.English ? Name.Substring(0, 3) : Name
            : string.Empty;

        [JsonProperty(PropertyName = "timing")]
        public List<BaseTiming>? Timing { get; set; }

        public bool IsTimingEmpty => Timing == null;

        public bool IsOpenNow
        {
            get
            {
                if (!IsTimingEmpty)
                {
                    foreach (var item in Timing)
                    {
                        if (item.FromDateTime <= DateTime.Now && item.ToDateTime >= DateTime.Now)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }

    public class EntityTitleResponse
    {
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string? Image { get; set; }
    }

    public class FavoriteEntity : Entity
    {
        [JsonProperty(PropertyName = "category_image")]
        public string? CategoryImage { get; set; }

        public List<ApiLinkResults>? EntityDetailsLinks { get; set; }
    }

    public class FavoriteEntityResponse : ApiLinks
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "entities")]
        public List<FavoriteEntity>? Entities { get; set; }
    }

    public class Filter
    {
        [JsonProperty(PropertyName = "title")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}
