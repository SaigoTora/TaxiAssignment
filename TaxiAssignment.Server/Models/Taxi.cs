namespace TaxiAssignment.Server.Models
{
	public class Taxi : PositionedEntity
	{
		public Taxi(int id, double latitude, double longitude)
			: base(id, latitude, longitude)
		{ }
	}
}