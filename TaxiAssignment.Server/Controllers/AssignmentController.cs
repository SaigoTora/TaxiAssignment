using Microsoft.AspNetCore.Mvc;

using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AssignmentController : ControllerBase
	{
		private readonly IGenerateDataService _generateDataService;

		public AssignmentController(IGenerateDataService generateDataService)
		{
			_generateDataService = generateDataService;
		}

		[HttpPost("generate-data")]
		public IActionResult GenerateData([FromBody] GenerateDataRequest request)
		{
			AssignmentData assignmentData = _generateDataService.GenerateData(request);
			return Ok(assignmentData);
		}
	}
}