namespace TaxiAssignment.Server.Models
{
	public class Car
	{
		public string Class { get; private set; }
		public string Brand { get; private set; }
		public string LicensePlate { get; private set; }
		public string Color { get; private set; }
		public int SeatsCount { get; private set; }

		public Car(string @class, string brand, string licensePlate, string color, int seatsCount)
		{
			Class = @class;
			Brand = brand;
			LicensePlate = licensePlate;
			Color = color;
			SeatsCount = seatsCount;
		}
	}
}