using Domain.Entities;
using Domain.Exceptions;

namespace DatesOverlapTest
{
    public class BookingTests
    {
        [Fact]
        public void CreateBooking_ShouldThrowOverlappingBookingException_WhenDatesOverlap()
        {
            // Arrange
            var apartmentId = Guid.NewGuid();

            var existingBookings = new List<Booking>
            {
                Booking.Create(
                    guestId: Guid.NewGuid(),
                    apartmentId: apartmentId,
                    stayStart: new DateOnly(2025, 10, 10),
                    stayEnd: new DateOnly(2025, 10, 15),
                    price: 1000,
                    existingBookings: new List<Booking>())
            };

            // Act & Assert
            Assert.Throws<OverlappingBookingException>(() =>
                Booking.Create(
                    guestId: Guid.NewGuid(),
                    apartmentId: apartmentId,
                    stayStart: new DateOnly(2025, 10, 14), // Overlaps with existing
                    stayEnd: new DateOnly(2025, 10, 18),
                    price: 1200,
                    existingBookings: existingBookings));
        }


        [Fact]
        public void CreateBooking_ShouldThrowArgumentException_WhenStayStartIsInThePast()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var yesterday = today.AddDays(-1);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                Booking.Create(
                    guestId: Guid.NewGuid(),
                    apartmentId: Guid.NewGuid(),
                    stayStart: yesterday,
                    stayEnd: today,
                    price: 1000,
                    existingBookings: new List<Booking>()));

            Assert.Equal("Stay cannot begin in the past.", ex.Message);
        }


        [Fact]
        public void CreateBooking_ShouldThrowArgumentException_WhenEndDateIsBeforeStartDate()
        {
            // Arrange
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                Booking.Create(
                    guestId: Guid.NewGuid(),
                    apartmentId: Guid.NewGuid(),
                    stayStart: tomorrow,
                    stayEnd: today, // Invalid: end before start
                    price: 1000,
                    existingBookings: new List<Booking>()));

            Assert.Equal("Stay end must be after stay start.", ex.Message);
        }



    }




}