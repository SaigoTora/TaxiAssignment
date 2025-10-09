namespace TaxiAssignment.Server.Models
{
	public class AssignmentData
	{
		public Taxi[] Taxis { get; private set; }
		public Client[] Clients { get; private set; }
		public double[,] Distances { get; private set; }

		public AssignmentData(Taxi[] taxis, Client[] clients, double[,] distances)
		{
			Taxis = taxis;
			Clients = clients;
			Distances = distances;
		}
	}
}