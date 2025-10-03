namespace TaxiAssignment.Server.Models
{
	public class AssignmentData
	{
		public Taxi[] Taxis { get; private set; }
		public Client[] Clients { get; private set; }

		public AssignmentData(Taxi[] taxis, Client[] clients)
		{
			Taxis = taxis;
			Clients = clients;
		}
	}
}