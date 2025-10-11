using System.Globalization;
using System.Reflection;

using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class GeoPointsService : IGeoPointsService
	{
		private record CityRoadData(string FileName, List<GeoLocation> Points);
		private record CityBounds(double MinLatitude, double MaxLatitude, double MinLongitude,
			double MaxLongitude);


		private readonly Random _random;
		private readonly Dictionary<City, CityRoadData> _cityRoadData;
		private readonly Dictionary<City, CityBounds> _cityBounds;
		private readonly List<GeoLocation> _kyivPoints, _kharkivPoints, _lvivPoints;

		public GeoPointsService(Random random)
		{
			_random = random;
			_kyivPoints = [];
			_kharkivPoints = [];
			_lvivPoints = [];

			_cityRoadData = new Dictionary<City, CityRoadData>
			{
				[City.Kyiv] = new("kyiv_roads.csv", _kyivPoints),
				[City.Kharkiv] = new("kharkiv_roads.csv", _kharkivPoints),
				[City.Lviv] = new("lviv_roads.csv", _lvivPoints)
			};

			_cityBounds = new Dictionary<City, CityBounds>
			{
				[City.Kyiv] = new(50.32, 50.58, 30.18, 31),
				[City.Kharkiv] = new(49.86, 50.145, 36.07, 36.44),
				[City.Lviv] = new(49.75, 49.91, 23.88, 24.16)
			};
		}

		public GeoLocation[] GetRandomPointsOnRoad(City city, int count)
		{
			if (!_cityRoadData.TryGetValue(city, out CityRoadData? roadData))
				throw new KeyNotFoundException($"Road data for city '{city}' was not found.");

			List<GeoLocation> roadPoints = roadData.Points;
			if (roadData.Points.Count == 0)
				LoadPointsFromFile(roadData.FileName, roadPoints);

			if (roadPoints.Count == 0)
				throw new InvalidOperationException($"No road points loaded from file " +
					$"'{roadData.FileName}' for city {city}");
			if (roadPoints.Count < count)
				throw new InvalidOperationException($"Requested {count} points, but only " +
					$"{roadPoints.Count} points available for city {city}");

			HashSet<GeoLocation> randomPoints = [];
			while (randomPoints.Count < count)
			{
				GeoLocation point = roadPoints[_random.Next(roadPoints.Count)];
				randomPoints.Add(point);
			}

			return [.. randomPoints];
		}
		public GeoLocation[] GetRandomPointsInCity(City city, int count)
		{
			if (!_cityBounds.TryGetValue(city, out CityBounds? bounds))
				throw new KeyNotFoundException($"Geographical bounds for city '{city}' " +
					$"were not found.");

			HashSet<GeoLocation> randomPoints = [];

			while (randomPoints.Count < count)
			{
				double latitude = GetRandomDouble(bounds.MinLatitude, bounds.MaxLatitude);
				double longitude = GetRandomDouble(bounds.MinLongitude, bounds.MaxLongitude);
				randomPoints.Add(new GeoLocation(latitude, longitude));
			}

			return [.. randomPoints];
		}

		private static void LoadPointsFromFile(string fileName, List<GeoLocation> points)
		{
			const char SEPARATOR = ',';
			const string FOLDER_NAME = "Data";

			string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.FullName,
				Assembly.GetExecutingAssembly().GetName().Name!, FOLDER_NAME);
			string filePath = Path.Combine(path, fileName);
			points.Clear();

			foreach (string line in File.ReadLines(filePath).Skip(1))
			{
				string[] parts = line.Split(SEPARATOR);
				if (parts.Length < 2) continue;

				if (double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture,
					out double latitude) &&
					double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture,
					out double longitude))
				{
					points.Add(new GeoLocation(latitude, longitude));
				}
			}
		}
		private double GetRandomDouble(double min, double max)
			=> _random.NextDouble() * (max - min) + min;
	}
}