namespace TaxiAssignment.Server.Services
{
	public class AuctionFixedEpsilonService : AuctionAssignmentService
	{
		protected override double[,] CreateSquareMatrix(double[,] costs, bool findMax)
		{
			int n = costs.GetLength(0), m = costs.GetLength(1);
			if (n == m)
				return costs;

			int maxLength = Math.Max(n, m);
			double[,] result = new double[maxLength, maxLength];
			double fillValue = costs[0, 0];

			if (findMax)
				for (int i = 0; i < costs.GetLength(0); i++)
					for (int j = 0; j < costs.GetLength(1); j++)
					{
						result[i, j] = costs[i, j];
						fillValue = Math.Min(fillValue, costs[i, j]);
					}
			else
				for (int i = 0; i < costs.GetLength(0); i++)
					for (int j = 0; j < costs.GetLength(1); j++)
					{
						result[i, j] = costs[i, j];
						fillValue = Math.Max(fillValue, costs[i, j]);
					}

			if (costs.GetLength(0) > costs.GetLength(1))
			{// If there are more rows than columns
				for (int i = 0; i < costs.GetLength(0); i++)
					for (int j = costs.GetLength(1); j < costs.GetLength(0); j++)
						result[i, j] = fillValue;
			}
			else
			{// If there are more columns than rows
				for (int i = costs.GetLength(0); i < costs.GetLength(1); i++)
					for (int j = 0; j < costs.GetLength(1); j++)
						result[i, j] = fillValue;
			}

			return result;
		}
		protected override double CalculateEpsilon(double[,] costs, int n) => 1.0 / (n + 1);
	}
}