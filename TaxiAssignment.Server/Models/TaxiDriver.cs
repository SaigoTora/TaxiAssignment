namespace TaxiAssignment.Server.Models
{
	public class TaxiDriver : Person
	{
		public TaxiDriver(int id, Location location, string name, string surname, int? age,
			string phoneNumber)
			: base(id, location, name, surname, age, phoneNumber)
		{ }
	}
}