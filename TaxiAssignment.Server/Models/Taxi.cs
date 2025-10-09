namespace TaxiAssignment.Server.Models
{
	public class Taxi : PositionedEntity
	{
		public Taxi(int id, Location location)
			: base(id, location)
		{ }
	}
}