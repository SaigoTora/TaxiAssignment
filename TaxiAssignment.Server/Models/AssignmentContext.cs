namespace TaxiAssignment.Server.Models
{
	public class AssignmentContext
	{
		public TaxiDriver[] TaxiDrivers { get; private set; }
		public Client[] Clients { get; private set; }
		public double[,] Distances { get; private set; }

		public AssignmentContext(TaxiDriver[] taxiDrivers, Client[] clients, double[,] distances)
		{
			TaxiDrivers = taxiDrivers;
			Clients = clients;
			Distances = distances;
		}
	}
}