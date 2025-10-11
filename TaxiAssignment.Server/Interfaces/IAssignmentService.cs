namespace TaxiAssignment.Server.Interfaces
{
	public interface IAssignmentService
	{
		/// <summary>
		/// Finds an optimal assignment of tasks to agents using the given cost matrix.
		/// </summary>
		/// <param name="costs">Matrix of assignment costs.</param>
		/// <param name="findMax">True to maximize total cost, false to minimize.</param>
		/// <returns>Array mapping each agent to its assigned task.</returns>
		int[] Solve(double[,] costs, bool findMax);
	}
}