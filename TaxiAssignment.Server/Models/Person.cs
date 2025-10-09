namespace TaxiAssignment.Server.Models
{
	public class Person : BaseEntity
	{
		public Location Location { get; private set; }
		public string Name { get; private set; }
		public string Surname { get; private set; }
		public int Age { get; private set; }
		public string PhoneNumber { get; private set; }

		public Person(int id, Location location, string name, string surname, int age,
			string phoneNumber)
		{
			SetId(id);

			Location = location;
			Name = name;
			Surname = surname;
			Age = age;
			PhoneNumber = phoneNumber;
		}
	}
}