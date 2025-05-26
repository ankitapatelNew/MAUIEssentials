using System;
using MAUIEssentials.AppCode.Helpers;
using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class BookingMessageResponse : ApiLinks
	{
		[JsonProperty(PropertyName = "booking_messages")]
		public List<BookingMessage>? BookingMessages { get; set; }

		[JsonProperty(PropertyName = "total_count")]
		public int TotalCount { get; set; }
	}

	public class BookingMessage
	{
		[JsonProperty(PropertyName = "sender")]
		public string? Sender { get; set; }

		[JsonProperty(PropertyName = "is_Owner")]
		public bool IsOwner { get; set; }

		[JsonProperty(PropertyName = "message")]
		public string? Message { get; set; }

		[JsonProperty(PropertyName = "date_time_created")]
		public DateTime DateCreated { get; set; }

		public string Time => DateCreated.ToString("hh:mm tt", CommonUtils.CurrentCulture).ConvertNumerals().ToLower();

		public string ShortMonthName => CommonUtils.GetMonthName(DateCreated);
		public string DateString => $"{DateCreated.ToString("dd").ConvertNumerals()} {ShortMonthName} {DateCreated.Year.ToString().ConvertNumerals()} {Time}";
	}
}
