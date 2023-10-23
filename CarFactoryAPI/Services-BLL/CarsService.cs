using CarAPI.Entities;
using CarAPI.Models;
using CarAPI.Payment;
using CarAPI.Repositories_DAL;

namespace CarAPI.Services_BLL
{
    public class CarsService : ICarsService
    {
        private readonly ICarsRepository _carsRepository;
		private readonly IOwnersRepository _ownerRepository;
		private readonly ICashService _cashService;
		public CarsService(ICarsRepository carsRepository, IOwnersRepository ownersRepository,
			ICashService cashServic)
        {
            _carsRepository = carsRepository;
            _ownerRepository = ownersRepository;
            _cashService = cashServic;
        }
        public List<Car> GetAll()
        {
            var cars = _carsRepository.GetAllCars();
            return cars;
        }

        public Car GetCarById(int id)
        {
            var car = _carsRepository.GetCarById(id);
            /// Logic on the car
            if (car == null) return null;
            return car;
        }

        public bool AddCar(Car car)
        {
            return _carsRepository.AddCar(car);
        }

        public bool Remove(int carId)
        {
            return _carsRepository.Remove(carId);
        }
		public string BuyCar(BuyCarInput input)
		{
			var car = _carsRepository.GetCarById(input.CarId);
			if (car == null)
				return "Car doesn't exist";
			if (car.Owner != null)
				return "Already sold";
			var owner = _ownerRepository.GetOwnerById(input.OwnerId);
			if (owner == null)
				return "Owner doesn't exist";
			if (owner.Car != null)
				return "solded";
			owner.Car = car;
			car.Owner = owner;

			var paymentResult = _cashService.Pay(input.Amount);
			return $"Successfull Car of Id: {input.CarId} is bought by {owner.Name} with payment result {paymentResult}";
		}
	}
}