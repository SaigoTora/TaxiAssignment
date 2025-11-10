using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;
using TaxiAssignment.Server.Services;

namespace TaxiAssignment.Tests
{
	enum AssignmentResultType
	{
		Exact,
		Approximate
	}


	public class AssignmentServiceTests
	{
		private const double EPSILON_PRECISION = 1.0;

		private static readonly HungarianAssignmentService _hungarian = new();
		private static readonly AuctionFixedEpsilonService _auctionFixed = new();
		private static readonly AuctionScaledEpsilonService _auctionScaled = new();

		private readonly Dictionary<IAssignmentService, Func<double[,], bool, AssignmentRequest>>
			_serviceToRequestMap = new()
			{
				[_hungarian] = (costs, findMax) => new AssignmentRequest(costs, findMax),
				[_auctionFixed] = (costs, findMax) => new AssignmentRequest(costs, findMax),
				[_auctionScaled] = (costs, findMax) => new AuctionScaledRequest(costs, findMax,
					EPSILON_PRECISION)
			};
		private readonly Dictionary<IAssignmentService, AssignmentResultType>
			_serviceResultTypeMap = new()
			{
				[_hungarian] = AssignmentResultType.Exact,
				[_auctionFixed] = AssignmentResultType.Exact,
				[_auctionScaled] = AssignmentResultType.Approximate
			};

		[Fact]
		public void Solve_5x5Matrix_Maximize_ReturnsExpectedAssignment()
		{
			// Arrange
			double[,] matrix = new double[5, 5];
			matrix[0, 0] = 7;
			matrix[0, 1] = 3;
			matrix[0, 2] = 6;
			matrix[0, 3] = 9;
			matrix[0, 4] = 5;

			matrix[1, 0] = 7;
			matrix[1, 1] = 5;
			matrix[1, 2] = 7;
			matrix[1, 3] = 5;
			matrix[1, 4] = 6;

			matrix[2, 0] = 7;
			matrix[2, 1] = 6;
			matrix[2, 2] = 8;
			matrix[2, 3] = 8;
			matrix[2, 4] = 9;

			matrix[3, 0] = 3;
			matrix[3, 1] = 1;
			matrix[3, 2] = 6;
			matrix[3, 3] = 5;
			matrix[3, 4] = 7;

			matrix[4, 0] = 2;
			matrix[4, 1] = 4;
			matrix[4, 2] = 9;
			matrix[4, 3] = 9;
			matrix[4, 4] = 5;

			bool findMax = true;

			// Act
			int[] expected = [3, 0, 1, 4, 2];
			foreach (var serviceRequest in _serviceToRequestMap)
			{
				IAssignmentService assignmentService = serviceRequest.Key;
				int[] result = assignmentService.Solve(serviceRequest.Value(matrix, findMax));

				// Assert
				Assert.True(IsAssignmentValid(assignmentService, matrix, expected, result));
			}
		}

		[Fact]
		public void Solve_3x2Matrix_Maximize_ReturnsExpectedAssignmentWithUnassigned()
		{
			// Arrange
			double[,] matrix = new double[3, 2];
			matrix[0, 0] = 2;
			matrix[0, 1] = 8;

			matrix[1, 0] = 4;
			matrix[1, 1] = 6;

			matrix[2, 0] = 5;
			matrix[2, 1] = 1;

			bool findMax = true;

			// Act
			int[] expected = [1, -1, 0];
			foreach (var serviceRequest in _serviceToRequestMap)
			{
				IAssignmentService assignmentService = serviceRequest.Key;
				int[] result = assignmentService.Solve(serviceRequest.Value(matrix, findMax));

				// Assert
				Assert.True(IsAssignmentValid(assignmentService, matrix, expected, result));
			}
		}

		[Fact]
		public void Solve_3x4Matrix_Maximize_ReturnsOneOfExpectedAssignments()
		{
			// Arrange
			double[,] matrix = new double[3, 4];
			matrix[0, 0] = 5;
			matrix[0, 1] = 8;
			matrix[0, 2] = 9;
			matrix[0, 3] = 5;

			matrix[1, 0] = 6;
			matrix[1, 1] = 7;
			matrix[1, 2] = 7;
			matrix[1, 3] = 6;

			matrix[2, 0] = 5;
			matrix[2, 1] = 8;
			matrix[2, 2] = 8;
			matrix[2, 3] = 6;

			bool findMax = true;

			// Act
			int[] firstExpected = [2, 0, 1];
			int[] secondExpected = [2, 3, 1];
			foreach (var serviceRequest in _serviceToRequestMap)
			{
				IAssignmentService assignmentService = serviceRequest.Key;
				int[] result = assignmentService.Solve(serviceRequest.Value(matrix, findMax));

				// Assert
				bool isValidFirstExpected = IsAssignmentValid(assignmentService, matrix,
					firstExpected, result);
				if (isValidFirstExpected)
					Assert.True(isValidFirstExpected);
				else
				{
					bool isValidSecondExpected = IsAssignmentValid(assignmentService, matrix,
						secondExpected, result);
					Assert.True(isValidSecondExpected);
				}
			}
		}

		[Fact]
		public void Solve_3x3Matrix_Minimize_ReturnsExpectedAssignment()
		{
			// Arrange
			double[,] matrix = new double[3, 3];
			matrix[0, 0] = 1;
			matrix[0, 1] = 2;
			matrix[0, 2] = 3;

			matrix[1, 0] = 3;
			matrix[1, 1] = 3;
			matrix[1, 2] = 3;

			matrix[2, 0] = 3;
			matrix[2, 1] = 3;
			matrix[2, 2] = 2;

			bool findMax = false;

			// Act
			int[] expected = [0, 1, 2];
			foreach (var serviceRequest in _serviceToRequestMap)
			{
				IAssignmentService assignmentService = serviceRequest.Key;
				int[] result = assignmentService.Solve(serviceRequest.Value(matrix, findMax));

				// Assert
				Assert.True(IsAssignmentValid(assignmentService, matrix, expected, result));
			}
		}

		[Fact]
		public void Solve_5x4Matrix_Minimize_ReturnsExpectedAssignmentWithUnassigned()
		{
			// Arrange
			double[,] matrix = new double[5, 4];
			matrix[0, 0] = 10;
			matrix[0, 1] = 19;
			matrix[0, 2] = 8;
			matrix[0, 3] = 15;

			matrix[1, 0] = 10;
			matrix[1, 1] = 18;
			matrix[1, 2] = 7;
			matrix[1, 3] = 17;

			matrix[2, 0] = 13;
			matrix[2, 1] = 16;
			matrix[2, 2] = 9;
			matrix[2, 3] = 14;

			matrix[3, 0] = 12;
			matrix[3, 1] = 19;
			matrix[3, 2] = 8;
			matrix[3, 3] = 19;

			matrix[4, 0] = 14;
			matrix[4, 1] = 17;
			matrix[4, 2] = 10;
			matrix[4, 3] = 19;

			bool findMax = false;

			// Act
			int[] expected = [0, 2, 3, -1, 1];
			foreach (var serviceRequest in _serviceToRequestMap)
			{
				IAssignmentService assignmentService = serviceRequest.Key;
				int[] result = assignmentService.Solve(serviceRequest.Value(matrix, findMax));

				// Assert
				Assert.True(IsAssignmentValid(assignmentService, matrix, expected, result));
			}
		}

		[Fact]
		public void Solve_3x4Matrix_Minimize_ReturnsExpectedAssignment()
		{
			// Arrange
			double[,] matrix = new double[3, 4];
			matrix[0, 0] = 2;
			matrix[0, 1] = 4;
			matrix[0, 2] = 8;
			matrix[0, 3] = 16;

			matrix[1, 0] = 4;
			matrix[1, 1] = 10;
			matrix[1, 2] = 5;
			matrix[1, 3] = 3;

			matrix[2, 0] = 3;
			matrix[2, 1] = 5;
			matrix[2, 2] = 1;
			matrix[2, 3] = 6;

			bool findMax = false;

			// Act
			int[] expected = [0, 3, 2];
			foreach (var serviceRequest in _serviceToRequestMap)
			{
				IAssignmentService assignmentService = serviceRequest.Key;
				int[] result = assignmentService.Solve(serviceRequest.Value(matrix, findMax));

				// Assert
				Assert.True(IsAssignmentValid(assignmentService, matrix, expected, result));
			}
		}


		private bool IsAssignmentValid(IAssignmentService assignmentService, double[,] matrix,
			int[] expected, int[] result)
		{
			if (_serviceResultTypeMap[assignmentService] == AssignmentResultType.Exact)
				return result.SequenceEqual(expected);
			else if (_serviceResultTypeMap[assignmentService] == AssignmentResultType.Approximate)
				return IsApproximateAssignmentValid(matrix, expected, result);

			return false;
		}
		private static bool IsApproximateAssignmentValid(double[,] matrix, int[] expectedResult,
			int[] actualResult)
		{
			const double ALLOWED_DEVIATION = 1;
			double expectedSum = 0, actualSum = 0;

			for (int i = 0; i < expectedResult.Length; i++)
			{
				if (expectedResult[i] != -1)
					expectedSum += matrix[i, expectedResult[i]];
				if (actualResult[i] != -1)
					actualSum += matrix[i, actualResult[i]];
			}

			// Check that the total assignment value is within allowed deviation
			return Math.Abs(expectedSum - actualSum) <= ALLOWED_DEVIATION;
		}
	}
}