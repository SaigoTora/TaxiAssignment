using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class GenerateDataService : IGenerateDataService
	{
		const double MIN_LATITUDE = 50.35;
		const double MAX_LATITUDE = 50.55;
		const double MIN_LONGITUDE = 30.30;
		const double MAX_LONGITUDE = 30.78;

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
			Location[] randomPoints = _roadPointsService.GetRandomPointsForCity(City.Kyiv, count);

			for (int i = 0; i < count; i++)
			{
				Location location = new(randomPoints[i].Latitude, randomPoints[i].Longitude);
				taxiDrivers[i] = new TaxiDriver(i + 1, location, "", "", null, "");
			}

			return taxiDrivers;
		}
		private Client[] GenerateRandomClients(int count)
		{
			Client[] entities = new Client[count];

			for (int i = 0; i < count; i++)
			{
				double latitude = GetRandomDouble(MIN_LATITUDE, MAX_LATITUDE);
				double longitude = GetRandomDouble(MIN_LONGITUDE, MAX_LONGITUDE);
				Location location = new(latitude, longitude);

				entities[i] = new Client(i + 1, location, "", "", null, "");
			}

			return entities;
		}
		private double GetRandomDouble(double min, double max)
			=> _random.NextDouble() * (max - min) + min;

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