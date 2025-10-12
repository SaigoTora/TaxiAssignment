using Microsoft.AspNetCore.Mvc;

using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AssignmentsController : ControllerBase
	{
		private readonly ILogger<AssignmentsController> _logger;
		private readonly IGenerateDataService _generateDataService;
		private readonly IAssignmentRunner _assignmentRunner;
		private readonly IAssignmentService _hungarianAssignmentService;
		private readonly IAssignmentService _auctionAssignmentService;

		public AssignmentsController(ILogger<AssignmentsController> logger,
			IGenerateDataService generateDataService, IAssignmentRunner assignmentRunner,
			[FromKeyedServices("hungarian")] IAssignmentService hungarianAssignmentService,
			[FromKeyedServices("auction")] IAssignmentService auctionAssignmentService)
		{
			_logger = logger;
			_generateDataService = generateDataService;
			_assignmentRunner = assignmentRunner;
			_hungarianAssignmentService = hungarianAssignmentService;
			_auctionAssignmentService = auctionAssignmentService;
		}

		[HttpPost("generate-data")]
		public IActionResult GenerateData([FromBody] GenerateDataRequest request)
		{
			AssignmentData assignmentData = _generateDataService.GenerateData(request);
			return Ok(assignmentData);
		}

		[HttpPost("hungarian")]
		public IActionResult AssignHungarian([FromBody] GenerateAssignRequest request)
		{
			AssignmentResult assignmentResult = _assignmentRunner.Run(_hungarianAssignmentService,
				request);
			return Ok(assignmentResult);
		}

		[HttpPost("auction")]
		public IActionResult AssignAuction([FromBody] GenerateAssignRequest request)
		{
			AssignmentResult assignmentResult = _assignmentRunner.Run(_hungarianAssignmentService,
				request);
			return Ok(assignmentResult);
		}
	}
}