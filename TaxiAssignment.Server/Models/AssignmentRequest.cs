namespace TaxiAssignment.Server.Models
{
	public class AssignmentRequest
	{
		public double[,] Costs { get; private set; }
		public bool FindMax { get; private set; }

		public AssignmentRequest(double[,] costs, bool findMax)
		{
			Costs = costs;
			FindMax = findMax;
		}
	}
}