namespace TaxiAssignment.Server.Models
{
	public class Client : PositionedEntity
	{
		public Client(int id, Location location)
			: base(id, location)
		{ }
	}
}