using Application;

namespace Application
{
    public class Booking
    {

        public Guid BookingId { get; set; } //Globally Unique Identifie
        public Guid GuestId { get; set; }
        public Guid ApartmentId { get; set; }
        public DateTime StayStart { get; set; }
        public DateTime StayEnd { get; set; }
        public int Price { get; set; }
        public bool Payment { get; set; }
        public bool Booked { get; set; }
    }
}

