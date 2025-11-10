namespace TaxiAssignment.Server.Models
{
	public class AuctionScaledRequest : AssignmentRequest
	{
		public double EpsilonPrecision { get; private set; }// Range: [0..1]

		public AuctionScaledRequest(double[,] costs, bool findMax, double epsilonPrecision)
			: base(costs, findMax)
		{
			EpsilonPrecision = epsilonPrecision;
		}
	}
}