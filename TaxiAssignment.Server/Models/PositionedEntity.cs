namespace TaxiAssignment.Server.Models
{
	public class PositionedEntity : BaseEntity
	{
		public double Latitude { get; private set; }
		public double Longitude { get; private set; }

		public PositionedEntity(int id, double latitude, double longitude)
			: this(latitude, longitude)
		{
			SetId(id);
		}
		public PositionedEntity(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}
	}
}