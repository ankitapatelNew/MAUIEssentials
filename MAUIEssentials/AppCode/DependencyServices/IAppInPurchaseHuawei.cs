using MAUIEssentials.Models;
using Plugin.InAppBilling;
using InAppBillingPurchase = MAUIEssentials.Models.InAppBillingPurchase;

namespace MAUIEssentials.AppCode.DependencyServices
{
    public interface IAppInPurchaseHuawei
    {
        public Task<string> HuaweiSignInAsync();
        public Task<List<InAppBillingPurchase>> GetPurchasesAsync();
        public Task<List<SubscriptionPriceLevel>> GetProductInfoAsync(List<string> productIds);
        public Task<InAppBillingPurchase> PurchaseAsync(SubscriptionPriceLevel subscriptionModel, string developerPayload);
        public Task<InAppBillingPurchase> UpgradePurchasedSubscriptionAsync(SubscriptionPriceLevel subscriptionModel, string developerPayload);
    }

    public interface IAppInPurchase
    {
        public Task<Plugin.InAppBilling.InAppBillingPurchase> UpgradePurchasedSubscriptionAsync(string newProductId, string purchaseTokenOfOriginalSubscription, SubscriptionProrationMode prorationMode = SubscriptionProrationMode.ImmediateWithTimeProration, string? subOfferToken = null);
    }

    public class PurchaseDeveloperPayload
    {
        public string? UserId { get; set; }
    }
}