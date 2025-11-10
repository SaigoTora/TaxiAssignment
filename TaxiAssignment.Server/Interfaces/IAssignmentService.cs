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
	}
}