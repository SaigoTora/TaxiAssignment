using Newtonsoft.Json;

namespace TaxiAssignment.Server.Models
{
	public class Location
	{
		public double Latitude { get; private set; }
		public double Longitude { get; private set; }

		private const double DegToRad = Math.PI / 180.0;

		[JsonIgnore]
		public double LatitudeInRadians => Latitude * DegToRad;

		[JsonIgnore]
		public double LongitudeInRadians => Longitude * DegToRad;

		public Location()
		{ }
		public Location(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public override bool Equals(object? obj)
		{
			if (obj is not Location other)
				return false;

			return Math.Abs(Latitude - other.Latitude) < 1e-9 &&
				   Math.Abs(Longitude - other.Longitude) < 1e-9;
		}
		public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
	}
}