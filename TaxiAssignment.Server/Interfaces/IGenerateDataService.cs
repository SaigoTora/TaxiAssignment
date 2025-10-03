using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Interfaces
{
	public interface IGenerateDataService
	{
		/// <summary>
		/// Generates a list of clients and taxis based on the given request.
		/// </summary>
		/// <param name="request">The number of clients and taxis to generate.</param>
		/// <returns>An AssignmentData object containing the generated clients and taxis.</returns>
		AssignmentData GenerateData(GenerateDataRequest request);
	}
}