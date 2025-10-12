namespace TaxiAssignment.Server.Models
{
	public class AssignmentResult
	{
		public TimeSpan ExecutionTime { get; private set; }
		public long MemoryUsedBytes { get; private set; }
		public int[] Assignment { get; private set; } = [];
		public double TotalDistanceMeters { get; private set; }

		public AssignmentResult(TimeSpan executionTime, long memoryUsedBytes, int[] assignment,
			double[,] distances)
		{
			ExecutionTime = executionTime;
			MemoryUsedBytes = memoryUsedBytes;
			Assignment = assignment;

			CalculateTotalDistance(assignment, distances);
		}

		private void CalculateTotalDistance(int[] assignment, double[,] distances)
		{
			for (int i = 0; i < assignment.Length; i++)
				if (assignment[i] != -1)
					TotalDistanceMeters += distances[i, assignment[i]];
		}
	}
}