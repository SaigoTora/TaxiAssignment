using System.Text;

using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class GenerateDataService : IGenerateDataService
	{
		private readonly string[] _maleNames =
		[
			"Oleksandr", "Dmytro", "Andriy", "Serhiy", "Volodymyr", "Mykhailo", "Ivan", "Petro",
			"Yuriy", "Oleh"
		];
		private readonly string[] _femaleNames =
		[
			"Anna", "Olena", "Kateryna", "Maria", "Iryna", "Tetyana", "Nataliya", "Svitlana",
			"Viktoriya", "Lyudmyla"
		];

		private readonly string[] _maleSurnames =
		[
			"Shevchenko", "Kovalchuk", "Bondarenko", "Tkachenko", "Havrylyuk", "Koval",
			"Hrytsenko", "Petrenko", "Romanenko", "Ivanenko"
		];
		private readonly string[] _femaleSurnames =
		[
			"Shevchenko", "Kovalchuk", "Bondarenko", "Tkachenko", "Havrylyuk", "Koval",
			"Hrytsenko", "Petrenko", "Romanenko", "Ivanenko"
		];

		private readonly string[] _phonePrefixes = ["50", "63", "66", "67", "68", "95", "99"];

		private readonly string[] _carClasses = ["Economy", "Comfort", "Premium"];
		private readonly string[] _carBrands =
		[
			"Toyota", "Hyundai", "Volkswagen", "Renault", "Skoda", "Kia", "Nissan", "Mercedes",
			"Ford", "Chevrolet", "Opel", "BMW", "Mitsubishi", "Mazda", "Honda", "Peugeot",
			"Citroen"
		];
		private readonly string[] _carColors = ["White", "Black", "Gray", "Silver", "Blue", "Red"];
		private readonly int[] _carSeatsCounts = [4, 4, 4, 4, 4, 5, 5, 6];

		private readonly string[] _carRegionCodes =
		[
			"AA","AB","AC","AE","AH","AI","AK","AM","AO","AP","AT","AX","BA","BB","BC","BE","BH",
			"BI","BK","BM","BO","BP","BT","BX","CA","CB","CE","CH","CI","CK","CM","CO","CP","CT",
			"CX","DA","DB","DC","DE","DH","DI","DK","DM","DO","DP","DT","DX","EA","EB","EC","EE",
			"EH","EI","EK","EM","EO","EP","ET","EX","HA","HB","HC","HE","HH","HI","HK","HM","HO",
			"HP","HT","HX","KA","KB","KC","KE","KH","KI","KK","KM","KO","KP","KT","KX","LA","LB",
			"LC","LE","LH","LI","LK","LM","LO","LP","LT","LX"
		];
		private readonly char[] _carLastLetters = ['A','B','C','E','H','K','M','O','P','T','X','Y',
			'I'];

		private readonly IRoadPointsService _roadPointsService;
		private readonly Random _random;

		public GenerateDataService(IRoadPointsService roadPointsService, Random random)
		{
			_roadPointsService = roadPointsService;
			_random = random;
		}

		public AssignmentData GenerateData(GenerateDataRequest request)
		{
			TaxiDriver[] taxiDrivers = GenerateRandomTaxiDrivers(request.TaxiDriversCount);
			Client[] clients = GenerateRandomClients(request.ClientCount);
			double[,] distances = CalculateDistances(taxiDrivers, clients);

			return new AssignmentData(taxiDrivers, clients, distances);
		}

		private TaxiDriver[] GenerateRandomTaxiDrivers(int count)
		{
			TaxiDriver[] taxiDrivers = new TaxiDriver[count];
			Location[] randomPoints = _roadPointsService.GetRandomPointsOnRoad(City.Kyiv, count);

			for (int i = 0; i < count; i++)
			{
				bool isMale = GenerateRandomBool();
				string name = GenerateRandomName(isMale);
				string surname = GenerateRandomSurname(isMale);
				int age = _random.Next(22, 55);
				string phoneNumber = GenerateRandomPhoneNumber();
				Car car = GenerateRandomCar();

				taxiDrivers[i] = new TaxiDriver(i + 1, randomPoints[i], name, surname, age,
					phoneNumber, car);
			}

			return taxiDrivers;
		}
		private Client[] GenerateRandomClients(int count)
		{
			Client[] clients = new Client[count];
			Location[] randomPoints = _roadPointsService.GetRandomPointsInCity(City.Kyiv, count);

			for (int i = 0; i < count; i++)
			{
				bool isMale = GenerateRandomBool();
				string name = GenerateRandomName(isMale);
				string surname = GenerateRandomSurname(isMale);
				int age = _random.Next(17, 55);
				string phoneNumber = GenerateRandomPhoneNumber();

				clients[i] = new Client(i + 1, randomPoints[i], name, surname, age, phoneNumber);
			}

			return clients;
		}

		private bool GenerateRandomBool() => _random.Next(2) == 0;
		private string GenerateRandomName(bool isMale)
		{
			if (isMale)
				return _maleNames[_random.Next(_maleNames.Length)];
			return _femaleNames[_random.Next(_femaleNames.Length)];
		}
		private string GenerateRandomSurname(bool isMale)
		{
			if (isMale)
				return _maleSurnames[_random.Next(_maleSurnames.Length)];
			return _femaleSurnames[_random.Next(_femaleSurnames.Length)];
		}
		private string GenerateRandomPhoneNumber()
		{
			StringBuilder builder = new($"+380(" +
				$"{_phonePrefixes[_random.Next(_phonePrefixes.Length)]})-");
			for (int i = 0; i < 7; i++)
			{
				builder.Append(_random.Next(10));
				if (i == 2 || i == 4)
					builder.Append('-');
			}

			return builder.ToString();
		}
		private Car GenerateRandomCar()
		{
			string carClass = _carClasses[_random.Next(_carClasses.Length)];
			string brand = _carBrands[_random.Next(_carBrands.Length)];
			string color = _carColors[_random.Next(_carColors.Length)];
			int seats = _carSeatsCounts[_random.Next(_carSeatsCounts.Length)];
			string licensePlate = GenerateRandomLicensePlate();

			return new Car(carClass, brand, licensePlate, color, seats);
		}
		private string GenerateRandomLicensePlate()
		{
			StringBuilder builder = new(_carRegionCodes[_random.Next(_carRegionCodes.Length)]);
			for (int i = 0; i < 4; i++)
				builder.Append(_random.Next(10));
			builder.Append(_carLastLetters[_random.Next(_carLastLetters.Length)]);
			builder.Append(_carLastLetters[_random.Next(_carLastLetters.Length)]);

			return builder.ToString();
		}

		private static double[,] CalculateDistances(TaxiDriver[] taxiDrivers, Client[] clients)
		{
			double[,] distances = new double[taxiDrivers.Length, clients.Length];

			for (int taxiIndex = 0; taxiIndex < taxiDrivers.Length; taxiIndex++)
				for (int clientIndex = 0; clientIndex < clients.Length; clientIndex++)
				{
					distances[taxiIndex, clientIndex] =
						CalculateDistance(taxiDrivers[taxiIndex].Location,
							clients[clientIndex].Location);
				}

			return distances;
		}
		public static double CalculateDistance(Location firstPoint, Location secondPoint)
		{
			const double EARTH_RADIUS_METERS = 6376500.0;

			double deltaLat = secondPoint.LatitudeInRadians - firstPoint.LatitudeInRadians;
			double deltaLng = secondPoint.LongitudeInRadians - firstPoint.LongitudeInRadians;

			// Haversine formula
			double haversineLat = Math.Pow(Math.Sin(deltaLat / 2), 2);
			double haversineLng = Math.Pow(Math.Sin(deltaLng / 2), 2);
			double h = haversineLat + Math.Cos(firstPoint.LatitudeInRadians) *
				Math.Cos(secondPoint.LatitudeInRadians) * haversineLng;

			double distance = 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
			return EARTH_RADIUS_METERS * distance;
		}
	}
}