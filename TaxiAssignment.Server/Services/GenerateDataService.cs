using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class GenerateDataService : IGenerateDataService
	{
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
				taxiDrivers[i] = new TaxiDriver(i + 1, randomPoints[i], "", "", null, "");
			}

			return taxiDrivers;
		}
		private Client[] GenerateRandomClients(int count)
		{
			Client[] clients = new Client[count];
			Location[] randomPoints = _roadPointsService.GetRandomPointsInCity(City.Kyiv, count);

			for (int i = 0; i < count; i++)
			{
				clients[i] = new Client(i + 1, randomPoints[i], "", "", null, "");
			}

			return clients;
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