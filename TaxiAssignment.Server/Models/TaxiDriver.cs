namespace TaxiAssignment.Server.Models
{
	public class TaxiDriver : Person
	{
		public Car Car { get; private set; }

		public TaxiDriver(int id, Location location, string name, string surname, int age,
			string phoneNumber, Car car)
			: base(id, location, name, surname, age, phoneNumber)
		{
			Car = car;
		}
	}
}