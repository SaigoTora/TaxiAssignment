using Newtonsoft.Json;

namespace TaxiAssignment.Server.Models
{
	public class GeoLocation
	{
		public double Latitude { get; private set; }
		public double Longitude { get; private set; }

		private const double DegToRad = Math.PI / 180.0;

		[JsonIgnore]
		public double LatitudeInRadians => Latitude * DegToRad;

		[JsonIgnore]
		public double LongitudeInRadians => Longitude * DegToRad;

		public GeoLocation()
		{ }
		public GeoLocation(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public override bool Equals(object? obj)
		{
			if (obj is not GeoLocation other)
				return false;

			const double EPSILON = 1e-9;

			return Math.Abs(Latitude - other.Latitude) < EPSILON &&
				   Math.Abs(Longitude - other.Longitude) < EPSILON;
		}
		public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
	}
}