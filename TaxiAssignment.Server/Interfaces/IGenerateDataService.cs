using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Interfaces
{
	public interface IGenerateDataService
	{
		/// <summary>
		/// Generates a list of clients and taxi drivers based on the given request.
		/// </summary>
		/// <param name="request">The number of clients and taxi drivers to generate.</param>
		/// <returns>An AssignmentContext object containing the generated clients and taxi drivers.</returns>
		AssignmentContext GenerateContext(GenerateDataRequest request);
	}
}