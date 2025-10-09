using System.Globalization;
using System.Reflection;

using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class RoadPointsService : IRoadPointsService
	{
		private readonly Random _random;
		private readonly Dictionary<City, (string fileName, List<Location> points)> _cityRoadData;
		private readonly List<Location> _kyivPoints;

		public RoadPointsService(Random random)
		{
			_random = random;
			_kyivPoints = [];

			_cityRoadData = new Dictionary<City, (string, List<Location>)>
			{
				[City.Kyiv] = ("kyiv_roads.csv", _kyivPoints)
			};
		}

		public Location[] GetRandomPointsForCity(City city, int count)
		{
			(string fileName, List<Location> roadPoints) = _cityRoadData[city];

			if (roadPoints.Count == 0)
				LoadPointsFromFile(fileName, roadPoints);

			if (roadPoints.Count == 0)
				throw new InvalidOperationException($"No road points loaded from file " +
					$"'{fileName}' for city {city}");
			if (roadPoints.Count < count)
				throw new InvalidOperationException($"Requested {count} points, but only " +
					$"{roadPoints.Count} points available for city {city}");

			HashSet<Location> randomPoints = [];
			while (randomPoints.Count < count)
			{
				Location point = roadPoints[_random.Next(roadPoints.Count)];
				randomPoints.Add(point);
			}

			return [.. randomPoints];
		}
		private static void LoadPointsFromFile(string fileName, List<Location> points)
		{
			const char SEPARATOR = '\t';
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
					points.Add(new Location(latitude, longitude));
				}
			}
		}
	}
}