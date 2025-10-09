namespace TaxiAssignment.Server.Models
{
	public class AssignmentData
	{
		public TaxiDriver[] TaxiDrivers { get; private set; }
		public Client[] Clients { get; private set; }
		public double[,] Distances { get; private set; }

		public AssignmentData(TaxiDriver[] taxiDrivers, Client[] clients, double[,] distances)
		{
			TaxiDrivers = taxiDrivers;
			Clients = clients;
			Distances = distances;
		}
	}
}