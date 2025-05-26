using System;
using MAUIEssentials.Models;
using Newtonsoft.Json;

namespace MAUIEssentials.AppCode.DependencyServices
{
    public interface IPayService
    {
        Task<string> PayWithApplePayOrGooglePay(ConfirmBookingResponse cardConfig, BillingAndShippingDetails billingShippingDetail, BookingDetailModel BookingDetail);
        Task<string> PayWithCard(ConfirmBookingResponse cardConfig, BillingAndShippingDetails billingShippingDetail, BookingDetailModel BookingDetail);
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

    public class PaymentDetail
    {
        public string? Token { get; set; }
        public string? TransactionTime { get; set; }
        public string? TransactionReference { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseMessage { get; set; }
        public string? ResponseStatus { get; set; }
        public string? CardType { get; set; }
        public string? CardScheme { get; set; }
        public string? PaymentDescription { get; set; }
    }
}