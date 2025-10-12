using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Interfaces
{
	public interface IAssignmentRunner
	{
		/// <summary>
		/// Executes the given assignment service on the provided assignment request.
		/// </summary>
		/// <param name="assignmentService">The assignment service that performs the distribution algorithm.</param>
		/// <param name="request">The input data containing the distances for the assignment.</param>
		/// <returns>The result of the assignment execution.</returns>
		AssignmentResult Run(IAssignmentService assignmentService, GenerateAssignRequest request);
	}
}