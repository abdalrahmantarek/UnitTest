using CarAPI.Entities;
using CarAPI.Models;
using CarAPI.Payment;
using CarAPI.Repositories_DAL;
using CarAPI.Services_BLL;
using CarFactoryAPITests.Fake;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CarsUnittest
{
	public class CarServicesTests : IDisposable
	{
		private readonly ITestOutputHelper testOutput;
		private Mock<IOwnersRepository> ownerRepoMock;
		private Mock<ICarsRepository> carRepoMock;
		private CarsService carsService;
		public CarServicesTests(ITestOutputHelper testOutput) 
		{
		     this.testOutput = testOutput;
			testOutput.WriteLine("StartTest");
			carRepoMock = new Mock<ICarsRepository>();
			ownerRepoMock = new Mock<IOwnersRepository>();
			carsService = new CarsService(
			   carRepoMock.Object,
			   ownerRepoMock.Object,
			   new CashService()
			   );
		}	
		public void Dispose()
		{
			testOutput.WriteLine("start clean up");
		}
		[Fact]
		public void BuyCar_CarHasOwner_AlreadySold()
		{
			CarsService carssService = new CarsService(
				new StupCarsWithOwnerRepo(),
				new StupOwnerWithoutCarRepo(),
				new CashService()
				);
			BuyCarInput buyCarInput = new()
			{
				CarId = 2,
				OwnerId = 1,
				Amount = 100
			};
			var result = carsService.BuyCar(buyCarInput);
			Assert.Equal("Car doesn't exist", result);
			testOutput.WriteLine(" car is exist");
		}


		[Fact]
		public void BuyCar_NewCarNewOwner_Sucessful()
		{
			Car car = new Car() { Id = 10, Price = 100 };
			Owner owner = new Owner() { Id = 1, Name = "sayed" };
			carRepoMock.Setup(m => m.GetCarById(It.IsAny<int>())).Returns(car);
			ownerRepoMock.Setup(m => m.GetOwnerById(It.IsAny<int>())).Returns(owner);
			BuyCarInput buyCarInput = new() { Amount = 100, CarId = 10, OwnerId = 2 };
			var result = carsService.BuyCar(buyCarInput);
			Assert.StartsWith("Successfull", result);
		}
		[Fact]
		public void BuyCar_OwnerNotExist_NotExist()
		{
			Car car = new Car() { Id = 10, Price = 100 };
			Owner owner = null;
			carRepoMock.Setup(m => m.GetCarById(car.Id)).Returns(car);
			ownerRepoMock.Setup(m => m.GetOwnerById(It.IsAny<int>())).Returns(owner);
			BuyCarInput buyCarInput = new() { Amount = 100, CarId = 10, OwnerId = 2 };
			var result = carsService.BuyCar(buyCarInput);
			Assert.Equal("Owner doesn't exist", result);
		}

	}
}
