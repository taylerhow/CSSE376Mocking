using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Expedia;
using Rhino.Mocks;

namespace ExpediaTest
{
	[TestClass]
	public class CarTest
	{	
		private Car targetCar;
		private MockRepository mocks;
		
		[TestInitialize]
		public void TestInitialize()
		{
			targetCar = new Car(5);
			mocks = new MockRepository();
		}
		
		[TestMethod]
		public void TestThatCarInitializes()
		{
			Assert.IsNotNull(targetCar);
		}	
		
		[TestMethod]
		public void TestThatCarHasCorrectBasePriceForFiveDays()
		{
			Assert.AreEqual(50, targetCar.getBasePrice()	);
		}
		
		[TestMethod]
		public void TestThatCarHasCorrectBasePriceForTenDays()
		{
            var target = new Car(10);
			Assert.AreEqual(80, target.getBasePrice());	
		}
		
		[TestMethod]
		public void TestThatCarHasCorrectBasePriceForSevenDays()
		{
			var target = new Car(7);
			Assert.AreEqual(10*7*.8, target.getBasePrice());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestThatCarThrowsOnBadLength()
		{
			new Car(-5);
		}

        [TestMethod()]
        public void TestThatCarDoesGetCarLocationFromDatabase()
        {
            IDatabase mockDB = mocks.DynamicMock<IDatabase>();

            String firstLocation = "2nd Floor, Space B7";
            String secondLocation = "4th Floor, Space E18";
            String noSuchCarID = "There is currently no car with that ID parked here";

            using (mocks.Ordered())
            {
                Expect.Call(mockDB.getCarLocation(342)).Return(firstLocation);
                Expect.Call(mockDB.getCarLocation(873)).Return(secondLocation);
                //Expect.Call(mockDB.getCarLocation(0)).Return(noSuchCarID);
            }

            mockDB.Stub(x => x.getCarLocation(Arg<int>.Is.Anything)).Return(noSuchCarID);

            mocks.ReplayAll();

            Car target = new Car(10);
            target.Database = mockDB;

            String result;

            result = target.getCarLocation(342);
            Assert.AreEqual(firstLocation, result);

            result = target.getCarLocation(873);
            Assert.AreEqual(secondLocation, result);

            result = target.getCarLocation(0);
            Assert.AreEqual(noSuchCarID, result);

            mocks.VerifyAll();
        }

        [TestMethod()]
        public void TestThatCarDoesGetMileageFromDatabase()
        {
            IDatabase mockDatabase = mocks.StrictMock<IDatabase>();

            Int32 fakeMileage = 75832956;

            Expect.Call(mockDatabase.Miles).PropertyBehavior();

            mocks.ReplayAll();

            mockDatabase.Miles = fakeMileage;
            var target = new Car(10);
            target.Database = mockDatabase;

            Int32 mileage = target.Mileage;
            Assert.AreEqual(mileage, fakeMileage);

            mocks.VerifyAll();
        }

        [TestMethod()]
        public void TestThatCarDoesGetMileageFromDatabaseUsingObjectMother()
        {
            IDatabase mockDatabase = mocks.StrictMock<IDatabase>();

            Int32 fakeMileage = 75832956;

            Expect.Call(mockDatabase.Miles).PropertyBehavior();

            mocks.ReplayAll();

            mockDatabase.Miles = fakeMileage;
            var target = ObjectMother.BMW();
            target.Database = mockDatabase;

            Int32 mileage = target.Mileage;
            Assert.AreEqual(mileage, fakeMileage);

            String nameFromObjectMother = "BMW S-Class";
            Assert.AreEqual(target.Name, nameFromObjectMother);

            mocks.VerifyAll();
        }
	}
}
