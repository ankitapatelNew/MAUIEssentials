using Plugin.InAppBilling;

namespace MAUIEssentials.Models
{
    public class InAppBillingPurchase
    {
        public string Id { get; set; }
        public string TransactionIdentifier { get; set; }
        public string ObfuscatedAccountId { get; set; }
        public string ObfuscatedProfileId { get; set; }
        public string OriginalTransactionIdentifier { get; set; }
        public DateTime TransactionDateUtc { get; set; }
        public string ProductId { get; set; }
        public IList<string> ProductIds { get; set; }
        public bool AutoRenewing { get; set; }
        public string PurchaseToken { get; set; }
        public PurchaseState State { get; set; }
        public ConsumptionState ConsumptionState { get; set; }
        public bool? IsAcknowledged { get; set; }
        public string ApplicationUsername { get; set; }
        public string Payload { get; set; }
        public string OriginalJson { get; set; }
        public string Signature { get; set; }
        public string ErrorMsg { get; set; }
        public string ErrorTitle { get; set; }
        public string BasePlanID { get; set; }
        public bool IsExpired { get; set; }
        public InAppBillingPurchase()
        {
        }
    }
    public enum MAUIPurchaseState
    {
        Purchased,
        Canceled,
        Purchasing,
        Failed,
        Restored,
        Deferred,
        PaymentPending,
        Unknown
    }

    public enum AppStoreId
    {
        Google = 1,
        Apple = 2,
        Huawie = 3
    }
}
