namespace TaxiAssignment.Server.Models
{
	public class PositionedEntity : BaseEntity
	{
		public Location Location { get; private set; }

		public PositionedEntity(int id, Location location)
			: this(location)
		{
			SetId(id);
		}
		public PositionedEntity(Location location)
		{
			Location = location;
		}
	}
}