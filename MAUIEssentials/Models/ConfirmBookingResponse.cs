using System.Globalization;
using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class ConfirmBookingResponse : ApiLinks
    {
        [JsonProperty(PropertyName = "booking_id")]
        public int bookingId { get; set; }

        [JsonProperty(PropertyName = "booking_request_status")]
        public string? Status { get; set; }

        [JsonProperty(PropertyName = "booking_request_status_id")]
        public int BookingStatusId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string? Date { get; set; }

        [JsonProperty(PropertyName = "booking_type")]
        public string? BookingType { get; set; }

        [JsonProperty(PropertyName = "option_name")]
        public string? OptionName { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string? Image { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string? Message { get; set; }

        [JsonProperty(PropertyName = "time")]
        public string? Time { get; set; }

        [JsonProperty(PropertyName = "payment_url")]
        public string? PaymentUrl { get; set; }

        [JsonProperty(PropertyName = "payment_parameters")]
        public PaymentData? PaymentParameters { get; set; }

        [JsonIgnore]
        public DateTime? DateWithTime => DateTime.ParseExact(Time ?? string.Empty, "M'/'dd'/'yyyy h:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"));

        [JsonIgnore]
        public PaymentCardModel? Card { get; set; }
    }

    public class PaymentData
    {
        [JsonProperty(PropertyName = "payment_config")]
        public PaymentConfiguration? paymentConfig { get; set; }

        [JsonProperty(PropertyName = "country_code")]
        public string? CountryCode { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }

        [JsonProperty(PropertyName = "payment_amount")]
        public string? PaymentAmount { get; set; }

        [JsonProperty(PropertyName = "payment_token")]
        public string? PaymentToken { get; set; }

        [JsonProperty(PropertyName = "payment_cart_Id")]
        public string? PaymentCartId { get; set; }

        [JsonProperty(PropertyName = "payment_description")]
        public string? PaymentDescription { get; set; }

        [JsonProperty(PropertyName = "token_last_transaction")]
        public string? TokenLastTransaction { get; set; }

        [JsonProperty(PropertyName = "billing_details")]
        public BillingAndShippingDetails? BillingDetails { get; set; }
    }

    public class BillingAndShippingDetails
    {
        [JsonProperty(PropertyName = "name")]
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string? Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string? Phone { get; set; }

        [JsonProperty(PropertyName = "address_line")]
        public string? AddressLine { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string? City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string? State { get; set; }

        [JsonProperty(PropertyName = "country_code")]
        public string? CountryCode { get; set; }

        [JsonProperty(PropertyName = "zipcode")]
        public string? Zip { get; set; }
    }

    public class PaymentConfiguration
    {
        [JsonProperty(PropertyName = "paytab_merchant_identifier")]
        public string? PaytabMerchantIdentifier { get; set; }

        [JsonProperty(PropertyName = "paytab_profile_id")]
        public string? PaytabProfileId { get; set; }

        [JsonProperty(PropertyName = "paytab_server_key")]
        public string? PaytabServer_Key { get; set; }

        [JsonProperty(PropertyName = "paytab_client_key")]
        public string? PaytabClienKey { get; set; }
    }
}