using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class AuctionFixedEpsilonService : AuctionAssignmentService
	{
		protected override double[,] CreateSquareMatrix(AssignmentRequest request)
		{
			double[,] costs = request.Costs;

			int n = costs.GetLength(0), m = costs.GetLength(1);
			if (n == m)
				return costs;

			int maxLength = Math.Max(n, m);
			double[,] result = new double[maxLength, maxLength];
			double fillValue = costs[0, 0];

			if (request.FindMax)
				for (int i = 0; i < n; i++)
					for (int j = 0; j < m; j++)
					{// Find minimum value
						result[i, j] = costs[i, j];
						if (costs[i, j] < fillValue)
							fillValue = costs[i, j];
					}
			else
				for (int i = 0; i < n; i++)
					for (int j = 0; j < m; j++)
					{// Find maximum value
						result[i, j] = costs[i, j];
						if (costs[i, j] > fillValue)
							fillValue = costs[i, j];
					}

			IAssignmentService.FillExtraCells(result, n, m, fillValue);

			return result;
		}
		protected override double CalculateEpsilon(double[,] costs, int n, double? epsilonPrecision)
			=> 1.0 / (n + 1);
	}
}