using System;
using System.Windows.Input;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class PaymentCard : ApiLinks
    {
        [JsonProperty(PropertyName = "cards")]
        public List<Card>? Cards { get; set; }
    }

    public class Card : ApiLinks
    {
        [JsonProperty(PropertyName = "name")]
        public string? name { get; set; }

        [JsonProperty(PropertyName = "last_digits")]
        public string? last_digits { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string? type { get; set; }

        [JsonProperty(PropertyName = "expiry_date")]
        public string? expiry_date { get; set; }

        [JsonProperty(PropertyName = "scheme")]
        public string? Scheme { get; set; }

        [JsonProperty(PropertyName = "card_mask")]
        public string? CardMask { get; set; }
    }

    public class PaymentCardModel : BaseNotifyPropertyChanged
    {
        Card? card;
        public Card? Card
        {
            get
            {
                return card;
            }
            set
            {
                card = value;
                OnPropertyChanged(nameof(Card));
                OnPropertyChanged(nameof(TypeImage));
                OnPropertyChanged(nameof(IsCard));
                OnPropertyChanged(nameof(Type));
            }
        }

        public string TypeImage => IsCard ? "ic_credit_card" : "ic_cash";
        public string? Type => IsCard ? Card?.name + " ( "+card?.CardMask+" )"  : (IsAddNewCard? LocalizationResources.addNewCard : LocalizationResources.cashText);
        public bool IsCard => Card != null;

        bool isBorderVisible;
        public bool IsBorderVisible
        {
            get
            {
                return isBorderVisible;
            }
            set
            {
                isBorderVisible = value;
                OnPropertyChanged(nameof(IsBorderVisible));
            }
        }

        bool isCardsAvailable;
        public bool IsCardsAvailable
        {
            get
            {
                return isCardsAvailable;
            }
            set
            {
                isCardsAvailable = value;
                OnPropertyChanged(nameof(isCardsAvailable));
            }
        }

        bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        bool isAddNewCard;
        public bool IsAddNewCard {
            get
            {
                return isAddNewCard;
            }
            set
            {
                isAddNewCard = value;
                OnPropertyChanged(nameof(IsAddNewCard));
            }
        }

        bool isAppleOrGooglePay;
        public bool IsAppleOrGooglePay
        {
            get
            {
                return isAppleOrGooglePay;
            }
            set
            {
                isAppleOrGooglePay = value;
                OnPropertyChanged(nameof(isAppleOrGooglePay));
                OnPropertyChanged(nameof(ApplePayTintColor));
                OnPropertyChanged(nameof(AppleGooglePayText));
            }
        }

        public Color ApplePayTintColor => DeviceInfo.Platform == DevicePlatform.iOS ? AppColorResources.blueColor.ToColor() : new Color();

        public string AppleGooglePayText => DeviceInfo.Platform == DevicePlatform.iOS ? LocalizationResources.applePay : LocalizationResources.googlePay;
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
                OnPropertyChanged(nameof(Image));
                OnPropertyChanged(nameof(ImageTintColor));
                OnPropertyChanged(nameof(TintColor));
            }
        }

        private ICommand? paymentTypeCommand;
        public ICommand? PaymentTypeCommand {
            get => paymentTypeCommand;
            set
            {
                paymentTypeCommand = value;
                OnPropertyChanged(nameof(PaymentTypeCommand));
            }
        }

        private ICommand? addNewCommand;
        public ICommand? AddNewCommand
        {
            get => addNewCommand;
            set
            {
                addNewCommand = value;
                OnPropertyChanged(nameof(AddNewCommand));
            }
        }

        public string Image => IsBusy? "" : IsSelected ? "ic_check_circle" : TypeImage;
        public Color ImageTintColor => IsSelected ? AppColorResources.greenColor.ToColor() : new Color();

        public Color TintColor => IsSelected ? AppColorResources.greenColor.ToColor() : AppColorResources.Gray91.ToColor();
        public string? TypeString => (Type == LocalizationResources.addNewCard) ? LocalizationResources.NewCardDetail : Card?.CardMask;

        public string CardImage => (Type == LocalizationResources.addNewCard) ? String.Empty: card.Scheme.Equals("Visa") ? "ic_visa" : "ic_master";

        public TextAlignment TextAlignmentTitle => (Settings.AppLanguage?.FlowDirection == FlowDirection.RightToLeft) ? TextAlignment.End : TextAlignment.Start;
    }
}
