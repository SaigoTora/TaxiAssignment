using System.ComponentModel.DataAnnotations;

using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Contracts
{
	public record GenerateDataRequest(
		City City,
		[Range(1, int.MaxValue, ErrorMessage = "Taxi drivers count must be at least 1")]
		int TaxiDriversCount,
		[Range(1, int.MaxValue, ErrorMessage = "Client count must be at least 1")]
		int ClientCount
	);
}