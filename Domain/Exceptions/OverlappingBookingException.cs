using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class OverlappingBookingException : Exception
    {
        public OverlappingBookingException()
            : base("Booking overlaps with an existing booking.") { }
    }
}
