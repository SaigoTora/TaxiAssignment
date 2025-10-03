namespace TaxiAssignment.Server.Models
{
	public class Client : PositionedEntity
	{
		public Client(int id, double latitude, double longitude)
			: base(id, latitude, longitude)
		{ }
	}
}