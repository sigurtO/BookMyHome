namespace Domain.Entities
{
    using Domain.Exceptions;

    public class Booking
    {
        public Guid BookingId { get; private set; }
        public Guid GuestId { get; private set; }
        public Guid ApartmentId { get; private set; }
        public DateOnly StayStart { get; private set; }
        public DateOnly StayEnd { get; private set; }
        public int Price { get; private set; }
        public bool Payment { get; private set; }

        private Booking() { } // For EF Core

        public static Booking Create(
            Guid guestId,
            Guid apartmentId,
            DateOnly stayStart,
            DateOnly stayEnd,
            int price,
            IEnumerable<Booking> existingBookings)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (stayStart < today)
                throw new ArgumentException("Stay cannot begin in the past.");

            if (stayEnd < stayStart)
                throw new ArgumentException("Stay end must be after stay start.");

            foreach (var existing in existingBookings.Where(b => b.ApartmentId == apartmentId))
            {
                if (DatesOverlap(stayStart, stayEnd, existing.StayStart, existing.StayEnd))
                    throw new OverlappingBookingException();
            }

            return new Booking
            {
                BookingId = Guid.NewGuid(),
                GuestId = guestId,
                ApartmentId = apartmentId,
                StayStart = stayStart,
                StayEnd = stayEnd,
                Price = price,
                Payment = false
            };
        }

        private static bool DatesOverlap(DateOnly start1, DateOnly end1, DateOnly start2, DateOnly end2)
        {
            // Inclusive overlap check
            return start1 <= end2 && start2 <= end1;
        }
    }
}
