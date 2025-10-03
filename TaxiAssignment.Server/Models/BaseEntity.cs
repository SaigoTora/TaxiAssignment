namespace TaxiAssignment.Server.Models
{
	public class BaseEntity
	{
		public int Id { get; protected set; }

		public void SetId(int id)
		{
			if (Id != 0)
				throw new InvalidOperationException("Id can only be set once.");

			if (id <= 0)
				throw new ArgumentException("Id must be greater than zero.", nameof(id));

			Id = id;
		}
	}
}