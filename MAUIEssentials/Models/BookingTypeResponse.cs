using System;

namespace MAUIEssentials.Models
{
    public class BookingTypeResponse
    {

    }

    public class BookingDetailModel
    {
        public Entity? Entity { get; set; }
        public AvailabileDaysTiming? Day { get; set; }
        public TimingModel? Timing { get; set; }
        public List<ApiLinkResults>? Links { get; set; }
        public List<BookingMessage>? Messages { get; set; }
        public string? SelectionId { get; set; }
    }
}