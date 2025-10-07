using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class GenerateDataService : IGenerateDataService
	{
		private readonly Random _random;
		public GenerateDataService(Random random)
		{
			_random = random;
		}

		public AssignmentData GenerateData(GenerateDataRequest request)
		{
			Taxi[] taxis = GetRandomTaxis(request.TaxiCount);
			Client[] clients = GetRandomClients(request.ClientCount);

			return new AssignmentData(taxis, clients);
		}

		private Taxi[] GetRandomTaxis(int count)
		{
			Taxi[] taxis = new Taxi[count];

			for (int i = 0; i < count; i++)
			{
				double latitude = GetRandomDouble(45, 51.5);
				double longitude = GetRandomDouble(25, 35);
				taxis[i] = new Taxi(i + 1, latitude, longitude);
			}

			return taxis;
		}
		private Client[] GetRandomClients(int count)
		{
			Client[] entities = new Client[count];

			for (int i = 0; i < count; i++)
			{
				double latitude = GetRandomDouble(45, 51.5);
				double longitude = GetRandomDouble(25, 35);
				entities[i] = new Client(i + 1, latitude, longitude);
			}

			return entities;
		}

		private double GetRandomDouble(double min, double max)
		{
			return _random.NextDouble() * (max - min) + min;
		}
	}
}