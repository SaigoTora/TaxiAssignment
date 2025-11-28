using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Interfaces
{
	public interface IAssignmentService
	{
		/// <summary>
		/// Finds an optimal assignment of tasks to agents using the given cost matrix.
		/// </summary>
		/// <param name="request">
		/// Assignment parameters including cost matrix and optional precision settings.
		/// </param>
		/// <returns>Array mapping each agent to its assigned task.</returns>
		int[] Solve(AssignmentRequest request);

		/// <summary>
		/// Fills the extra cells of a non-square matrix with a specified value
		/// to make it square. Only the cells outside the original dimensions are updated.
		/// </summary>
		/// <param name="costs">The matrix to fill. Must be non-null.</param>
		/// <param name="fillValue">The value to assign to the extra cells.</param>
		protected static void FillExtraCells(double[,] costs, double fillValue)
		{
			int n = costs.GetLength(0), m = costs.GetLength(1);
			if (n > m)
			{// If there are more rows than columns
				for (int i = 0; i < n; i++)
					for (int j = m; j < n; j++)
						costs[i, j] = fillValue;
			}
			else
			{// If there are more columns than rows
				for (int i = n; i < m; i++)
					for (int j = 0; j < m; j++)
						costs[i, j] = fillValue;
			}
		}
	}
}