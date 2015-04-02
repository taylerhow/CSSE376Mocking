
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Expedia;
using System.Reflection;

namespace ExpediaTest
{
	[TestClass]
	public class UserTest
	{
		private User target;
		private readonly DateTime StartDate = new DateTime(2009, 11, 1);
		private readonly DateTime EndDate = new DateTime(2009, 11, 30);
		
		[TestInitialize]
		public void TestInitialize()
		{
			target = new User("Bob Dole");
		}
		
		[TestMethod]
		public void TestThatUserInitializes()
		{
			Assert.AreEqual("Bob Dole", target.Name);
		}
		
		[TestMethod]
		public void TestThatUserHasZeroFrequentFlierMilesOnInit()
		{
			Assert.AreEqual(0, target.FrequentFlierMiles);
		}
		
		[TestMethod]
		public void TestThatUserCanBookEverything()
		{
			target.book(new Flight(StartDate, EndDate, 0), new Hotel(5), new Car(3));
			Assert.AreEqual(3, target.Bookings.Count);
		}
		
		[TestMethod]
		public void TestThatUserHasFrequentFlierMilesAfterBooking()
		{
			target.book(new Flight(StartDate, EndDate, 1), new Hotel(5), new Car(3));
			Assert.IsTrue(0 < target.FrequentFlierMiles);
			Assert.AreEqual(3, target.Bookings.Count);
		}
		
		[TestMethod]
		public void TestThatUserCanBookAOnlyFlight()
		{
			target.book(new Flight(StartDate, EndDate, 0));
			Assert.AreEqual(1, target.Bookings.Count);
		}
		
		[TestMethod]
		public void TestThatUserCanBookAHotalAndACar()
		{
			target.book(new Car(5), new Hotel(5));
			Assert.AreEqual(2, target.Bookings.Count);
		}
		
		[TestMethod]
		public void TestThatUserHasCorrectNumberOfFrequentFlyerMilesAfterOneFlight()
		{
			target.book(new Flight(StartDate, EndDate, 500));
			Assert.AreEqual(500, target.FrequentFlierMiles);
		}
		
		[TestMethod]
		public void TestThatUserTotalCostIsCorrect()
		{
			var flight = new Flight(StartDate, EndDate, 500);
			target.book(flight);	
			Assert.AreEqual(flight.getBasePrice(), target.Price);
		}
		
		[TestMethod]
		public void TestThatUserTotalCostIsCorrectWhenMoreThanFlights()
		{
			var car = new Car(5);
			var flight = new Flight(StartDate, EndDate, 500);
			target.book(flight);	
			target.book(car);
			Assert.AreEqual(flight.getBasePrice() + car.getBasePrice(), target.Price);
		}

        [TestMethod]
        public void TestThatUserDoesRemoveCarFromServiceLocatorWhenBooked()
        {
            ServiceLocator serviceLocator = new ServiceLocator();
            var carToBook = new Car(5);
            var remainingCar = new Car(7);
            serviceLocator.AddCar(carToBook);
            serviceLocator.AddCar(remainingCar);

            typeof(ServiceLocator).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(serviceLocator, serviceLocator);

            var target = new User("Bob");
            target.book(carToBook);

            Assert.AreEqual(1, ServiceLocator.Instance.AvailableCars.Count);
            Assert.AreSame(remainingCar, ServiceLocator.Instance.AvailableCars[0]);
        }

        [TestMethod]
        public void TestThatUserDoesRemoveFlightFromServiceLocatorWhenBooked()
        {
            ServiceLocator serviceLocator = new ServiceLocator();
            var flightToBook = new Flight(new DateTime(2008, 11, 1), new DateTime(2008, 11, 30), 500);
            var remainingFlight = new Flight(new DateTime(2009, 11, 1), new DateTime(2009, 11, 30), 800);
            serviceLocator.AddFlight(flightToBook);
            serviceLocator.AddFlight(remainingFlight);

            typeof(ServiceLocator).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(serviceLocator, serviceLocator);

            var target = new User("Tom");
            target.book(flightToBook);

            Assert.AreEqual(1, ServiceLocator.Instance.AvailableFlights.Count);
            Assert.AreSame(remainingFlight, ServiceLocator.Instance.AvailableFlights[0]);
        }
		
		[TestCleanup]
		public void TearDown()
		{
			target = null; // this is entirely unnecessary.. but I'm just showing a usage of the TearDown method here
		}
	}
}
