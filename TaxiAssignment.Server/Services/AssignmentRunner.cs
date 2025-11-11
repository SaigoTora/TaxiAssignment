using System.Diagnostics;

using TaxiAssignment.Server.Contracts;
using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class AssignmentRunner : IAssignmentRunner
	{
		public AssignmentResult Run(IAssignmentService assignmentService,
			GenerateAssignRequest request)
		{
			AssignmentRequest assignmentRequest;
			if (request is GenerateAssignWithEpsilonRequest epsilonRequest)
				assignmentRequest = new AuctionScaledRequest(request.Distances, false,
					epsilonRequest.EpsilonPrecision);
			else
				assignmentRequest = new AssignmentRequest(request.Distances, false);

			long memoryBefore = GC.GetTotalMemory(true), memoryAfter;
			Stopwatch sw = new();
			sw.Start();

			int[] assignment = assignmentService.Solve(assignmentRequest);

			sw.Stop();
			memoryAfter = GC.GetTotalMemory(false);
			long memoryUsedBytes = memoryAfter - memoryBefore;

			return new(sw.ElapsedMilliseconds, memoryUsedBytes, assignment, request.Distances);
		}
	}
}