namespace TaxiAssignment.Server.Models
{
	public class Client : Person
	{
		public Client(int id, Location location, string name, string surname, int age,
			string phoneNumber)
			: base(id, location, name, surname, age, phoneNumber)
		{ }
	}
}