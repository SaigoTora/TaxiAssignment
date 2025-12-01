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
		/// <param name="rowCount">The original number of rows in the matrix.</param>
		/// <param name="columnCount">The original number of columns in the matrix.</param>
		/// <param name="fillValue">The value to assign to the extra cells.</param>
		protected static void FillExtraCells(double[,] costs, int rowCount, int columnCount,
			double fillValue)
		{
			if (rowCount > columnCount)
			{// If there are more rows than columns
				for (int i = 0; i < rowCount; i++)
					for (int j = columnCount; j < rowCount; j++)
						costs[i, j] = fillValue;
			}
			else
			{// If there are more columns than rows
				for (int i = rowCount; i < columnCount; i++)
					for (int j = 0; j < columnCount; j++)
						costs[i, j] = fillValue;
			}
		}
	}
}