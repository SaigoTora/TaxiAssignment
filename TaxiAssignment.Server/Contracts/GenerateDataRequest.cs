using System.ComponentModel.DataAnnotations;

namespace TaxiAssignment.Server.Contracts
{
	public record GenerateDataRequest(
		[Range(1, int.MaxValue, ErrorMessage = "Taxi count must be at least 1")]
		int TaxiCount,
		[Range(1, int.MaxValue, ErrorMessage = "Client count must be at least 1")]
		int ClientCount
	);
}