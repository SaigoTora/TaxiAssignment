using System.Device.Location;

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
			Taxi[] taxis = GenerateRandomTaxis(request.TaxiCount);
			Client[] clients = GenerateRandomClients(request.ClientCount);
			double[,] distances = CalculateDistances(taxis, clients);

			return new AssignmentData(taxis, clients, distances);
		}

		private Taxi[] GenerateRandomTaxis(int count)
		{
			Taxi[] taxis = new Taxi[count];
			Location[] randomPoints = _roadPointsService.GetRandomPointsForCity(City.Kyiv, count);

			for (int i = 0; i < count; i++)
			{
				Location location = new(randomPoints[i].Latitude, randomPoints[i].Longitude);
				taxis[i] = new Taxi(i + 1, location);
			}

			return taxis;
		}
		private Client[] GenerateRandomClients(int count)
		{
			Client[] entities = new Client[count];

			for (int i = 0; i < count; i++)
			{
				double latitude = GetRandomDouble(MIN_LATITUDE, MAX_LATITUDE);
				double longitude = GetRandomDouble(MIN_LONGITUDE, MAX_LONGITUDE);
				Location location = new(latitude, longitude);

				entities[i] = new Client(i + 1, location);
			}

			return entities;
		}
		private static double[,] CalculateDistances(Taxi[] taxis, Client[] clients)
		{
			double[,] distances = new double[taxis.Length, clients.Length];

			GeoCoordinate taxiCoordinate = new();
			GeoCoordinate clientCoordinate = new();

			for (int taxiIndex = 0; taxiIndex < taxis.Length; taxiIndex++)
				for (int clientIndex = 0; clientIndex < clients.Length; clientIndex++)
				{
					taxiCoordinate.Latitude = taxis[taxiIndex].Location.Latitude;
					taxiCoordinate.Longitude = taxis[taxiIndex].Location.Longitude;
					clientCoordinate.Latitude = clients[clientIndex].Location.Latitude;
					clientCoordinate.Longitude = clients[clientIndex].Location.Longitude;

					distances[taxiIndex, clientIndex] = taxiCoordinate
						.GetDistanceTo(clientCoordinate);
				}

			return distances;
		}

		private double GetRandomDouble(double min, double max)
			=> _random.NextDouble() * (max - min) + min;
	}
}