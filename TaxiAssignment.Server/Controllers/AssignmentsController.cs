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
		private readonly IAssignmentService _auctionFixedAssignmentService;
		private readonly IAssignmentService _auctionScaledAssignmentService;

		public AssignmentsController(ILogger<AssignmentsController> logger,
			IGenerateDataService generateDataService, IAssignmentRunner assignmentRunner,
			[FromKeyedServices("hungarian")] IAssignmentService hungarianAssignmentService,
			[FromKeyedServices("auction-fixed")] IAssignmentService auctionFixedAssignmentService,
			[FromKeyedServices("auction-scaled")] IAssignmentService auctionScaledAssignmentService
			)
		{
			_logger = logger;
			_generateDataService = generateDataService;
			_assignmentRunner = assignmentRunner;
			_hungarianAssignmentService = hungarianAssignmentService;
			_auctionFixedAssignmentService = auctionFixedAssignmentService;
			_auctionScaledAssignmentService = auctionScaledAssignmentService;
		}

		[HttpPost("generate-data")]
		public IActionResult GenerateData([FromBody] GenerateDataRequest request)
		{
			AssignmentContext assignmentContext = _generateDataService.GenerateContext(request);
			return Ok(assignmentContext);
		}

		[HttpPost("hungarian")]
		public IActionResult AssignHungarian([FromBody] GenerateAssignRequest request)
		{
			AssignmentResult assignmentResult = _assignmentRunner.Run(_hungarianAssignmentService,
				request);
			return Ok(assignmentResult);
		}

		[HttpPost("auction-fixed")]
		public IActionResult AssignAuctionFixed([FromBody] GenerateAssignRequest request)
		{
			AssignmentResult assignmentResult = _assignmentRunner.Run(
				_auctionFixedAssignmentService, request);
			return Ok(assignmentResult);
		}

		[HttpPost("auction-scaled")]
		public IActionResult AssignAuctionScaled([FromBody] GenerateAssignRequest request)
		{
			AssignmentResult assignmentResult = _assignmentRunner.Run(
				_auctionScaledAssignmentService, request);
			return Ok(assignmentResult);
		}
	}
}