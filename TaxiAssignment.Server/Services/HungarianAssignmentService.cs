using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class HungarianAssignmentService : IAssignmentService
	{
		private enum Mask : byte
		{
			Empty = 0,
			Star = 1,
			Prime = 2
		}
		private enum HungarianStep
		{
			MarkStars = 1,
			PrimeAndCover = 2,
			BuildPath = 3,
			AdjustCosts = 4,
			Last = -1
		}
		private readonly record struct Location(int Row, int Column);


		private const int EMPTY_ROW_INDEX = -1;
		private const int EMPTY_COLUMN_INDEX = -1;
		private const double EPSILON = 1e-9;

		public int[] Solve(AssignmentRequest request)
		{
			ArgumentNullException.ThrowIfNull(request);
			ArgumentNullException.ThrowIfNull(request.Costs);

			double[,] costs;
			int n = request.Costs.GetLength(0), m = request.Costs.GetLength(1);

			if (n == m)// Clone the matrix to avoid modifying the original input
				costs = (double[,])request.Costs.Clone();
			else
				costs = CreateSquareMatrix(request.Costs);

			if (request.FindMax)
			{// If the assignment is resolved to the maximum
				for (int i = 0; i < costs.GetLength(0); i++)
				{
					double max = costs[i, 0];
					for (int j = 0; j < costs.GetLength(1); j++)
						max = Math.Max(max, costs[i, j]);

					for (int j = 0; j < costs.GetLength(1); j++)
						costs[i, j] = max - costs[i, j];
				}
			}

			int height = costs.GetLength(0);
			int width = costs.GetLength(1);

			for (int i = 0; i < height; i++)
			{// Row reduction
				double min = double.MaxValue;// Minimum value in the current row
				for (int j = 0; j < width; j++)
					min = Math.Min(min, costs[i, j]);

				for (int j = 0; j < width; j++)
					costs[i, j] -= min;
			}
			for (int i = 0; i < width; i++)
			{// Column reduction
				double min = double.MaxValue;// Minimum value in the current column
				for (int j = 0; j < height; j++)
					min = Math.Min(min, costs[j, i]);

				for (int j = 0; j < height; j++)
					costs[j, i] -= min;
			}

			Mask[,] masks = new Mask[height, width];
			bool[] rowsCovered = new bool[height];
			bool[] colsCovered = new bool[width];

			// Obtaining a mask, where we denote by the number 1 those elements that
			// are zero in the original matrix and do not have 0 at the intersection
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++)
					if (Math.Abs(costs[i, j]) < EPSILON && !rowsCovered[i] && !colsCovered[j])
					{
						masks[i, j] = Mask.Star;
						rowsCovered[i] = true;
						colsCovered[j] = true;
					}

			ClearCovers(rowsCovered, colsCovered);

			Location[] path = new Location[width * height];
			Location pathStart = default;
			HungarianStep step = HungarianStep.MarkStars;

			while (step != HungarianStep.Last)
			{
				if (step == HungarianStep.MarkStars)
					step = MarkInitialStars(masks, colsCovered);
				if (step == HungarianStep.PrimeAndCover)
					step = PrimeAndCover(costs, masks, rowsCovered, colsCovered, ref pathStart);
				if (step == HungarianStep.BuildPath)
					step = BuildAndConvertPath(masks, rowsCovered, colsCovered, path, pathStart);
				if (step == HungarianStep.AdjustCosts)
					step = AdjustCosts(costs, rowsCovered, colsCovered);
			}

			// Convert the mask matrix to the desired size
			Mask[,] newMasks = new Mask[n, m];
			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
					newMasks[i, j] = masks[i, j];

			// The resulting array, where the index is the worker and the value is the task
			const int UNKNOWN_TASK = -1;
			int[] agentsTasks = new int[newMasks.GetLength(0)];

			for (int i = 0; i < newMasks.GetLength(0); i++)
				for (int j = 0; j < newMasks.GetLength(1); j++)
				{
					if (newMasks[i, j] == Mask.Star)
					{
						agentsTasks[i] = j;
						break;
					}
					else if (j == newMasks.GetLength(1) - 1)
						agentsTasks[i] = UNKNOWN_TASK;// If the worker was not up to the task
				}

			return agentsTasks;
		}

		private static double[,] CreateSquareMatrix(double[,] matrix)
		{
			if (matrix.GetLength(0) == matrix.GetLength(1))
				return matrix;

			int maxLength = Math.Max(matrix.GetLength(0), matrix.GetLength(1));
			double[,] result = new double[maxLength, maxLength];
			double max = matrix[0, 0];

			for (int i = 0; i < matrix.GetLength(0); i++)// Writing existing elements into an array
				for (int j = 0; j < matrix.GetLength(1); j++)// and finding the maximum
				{
					result[i, j] = matrix[i, j];
					max = Math.Max(max, matrix[i, j]);
				}

			if (matrix.GetLength(0) > matrix.GetLength(1))
			{// If there are more rows than columns
				for (int i = 0; i < matrix.GetLength(0); i++)
					for (int j = matrix.GetLength(1); j < matrix.GetLength(0); j++)
						result[i, j] = max;
			}
			else
			{// If there are more columns than rows
				for (int i = matrix.GetLength(0); i < matrix.GetLength(1); i++)
					for (int j = 0; j < matrix.GetLength(1); j++)
						result[i, j] = max;
			}
			return result;
		}

		private static HungarianStep MarkInitialStars(Mask[,] masks, bool[] colsCovered)
		{
			// Mark the data in colsCovered
			for (int i = 0; i < masks.GetLength(0); i++)
				for (int j = 0; j < masks.GetLength(1); j++)
					if (masks[i, j] == Mask.Star)
						colsCovered[j] = true;

			// Find the number of crossed out columns
			int colsCoveredCount = 0;
			for (int j = 0; j < colsCovered.Length; j++)
				if (colsCovered[j])
					colsCoveredCount++;

			// If all columns are crossed out, this is the last step in the cycle.
			if (colsCoveredCount == colsCovered.Length)
				return HungarianStep.Last;

			return HungarianStep.PrimeAndCover;
		}
		private static HungarianStep PrimeAndCover(double[,] costs, Mask[,] masks, bool[] rowsCovered,
			bool[] colsCovered, ref Location pathStart)
		{
			while (true)
			{
				Location location = FindZero(costs, rowsCovered, colsCovered);
				if (location.Row == EMPTY_ROW_INDEX)// If no uncrossed zero was found
					return HungarianStep.AdjustCosts;

				masks[location.Row, location.Column] = Mask.Prime;

				int indexCol = FindIndexInRow(masks, location.Row);
				if (indexCol != EMPTY_COLUMN_INDEX)
				{// If the index was found
					rowsCovered[location.Row] = true;
					colsCovered[indexCol] = false;
				}
				else
				{
					pathStart = location;
					return HungarianStep.BuildPath;
				}
			}
		}
		private static HungarianStep BuildAndConvertPath(Mask[,] masks, bool[] rowsCovered,
			bool[] colsCovered, Location[] path, Location pathStart)
		{
			int pathIndex = 0;
			path[0] = pathStart;

			while (true)
			{
				int row = FindIndexInColumn(masks, path[pathIndex].Column);
				if (row == EMPTY_ROW_INDEX)
					break;

				pathIndex++;
				path[pathIndex] = new Location(row, path[pathIndex - 1].Column);

				int col = FindPrimeInRow(masks, path[pathIndex].Row);

				pathIndex++;
				path[pathIndex] = new Location(path[pathIndex - 1].Row, col);
			}

			ConvertPath(masks, path, pathIndex + 1);
			ClearCovers(rowsCovered, colsCovered);
			ClearPrimes(masks);

			return HungarianStep.MarkStars;
		}
		private static HungarianStep AdjustCosts(double[,] costs, bool[] rowsCovered,
			bool[] colsCovered)
		{
			double minValue = FindMinimum(costs, rowsCovered, colsCovered);

			// Add or subtract the minimum depending on the strikethrough
			for (int i = 0; i < costs.GetLength(0); i++)
				for (int j = 0; j < costs.GetLength(1); j++)
				{
					if (rowsCovered[i])
						costs[i, j] += minValue;
					if (!colsCovered[j])
						costs[i, j] -= minValue;
				}

			return HungarianStep.PrimeAndCover;
		}

		private static double FindMinimum(double[,] costs, bool[] rowsCovered, bool[] colsCovered)
		{// Method that finds the minimum value among NOT crossed out elements
			double minValue = double.MaxValue;

			for (int i = 0; i < costs.GetLength(0); i++)
				for (int j = 0; j < costs.GetLength(1); j++)
					if (!rowsCovered[i] && !colsCovered[j])
						minValue = Math.Min(minValue, costs[i, j]);

			return minValue;
		}
		private static int FindIndexInRow(Mask[,] masks, int row)
		{// The method returns the column index if there is a unit in a particular row
			for (int j = 0; j < masks.GetLength(1); j++)
				if (masks[row, j] == Mask.Star)
					return j;

			return EMPTY_COLUMN_INDEX;// If not found
		}
		private static int FindIndexInColumn(Mask[,] masks, int col)
		{// The method returns the row index if there is a unit in a certain column
			for (int i = 0; i < masks.GetLength(0); i++)
				if (masks[i, col] == Mask.Star)
					return i;

			return EMPTY_ROW_INDEX;// If not found
		}
		private static int FindPrimeInRow(Mask[,] masks, int row)
		{// The method returns the column index if there is a two in a given row
			for (int j = 0; j < masks.GetLength(1); j++)
				if (masks[row, j] == Mask.Prime)
					return j;

			return EMPTY_COLUMN_INDEX;// If not found
		}
		private static Location FindZero(double[,] costs, bool[] rowsCovered, bool[] colsCovered)
		{// The method returns the position of the first uncrossed zero
			for (int i = 0; i < costs.GetLength(0); i++)
				for (int j = 0; j < costs.GetLength(1); j++)
					if (Math.Abs(costs[i, j]) < EPSILON && !rowsCovered[i] && !colsCovered[j])
						return new Location(i, j);

			// If zero was not found
			return new Location(EMPTY_ROW_INDEX, EMPTY_COLUMN_INDEX);
		}

		private static void ConvertPath(Mask[,] masks, Location[] path, int pathLength)
		{// A method that modifies the data in the masks matrix
			int row, column;
			for (int i = 0; i < pathLength; i++)
			{
				row = path[i].Row;
				column = path[i].Column;

				if (masks[row, column] == Mask.Star)
					masks[row, column] = Mask.Empty;

				else if (masks[row, column] == Mask.Prime)
					masks[row, column] = Mask.Star;
			}
		}
		private static void ClearPrimes(Mask[,] masks)
		{// Method that writes 0 for elements with value 2
			for (var i = 0; i < masks.GetLength(0); i++)
				for (var j = 0; j < masks.GetLength(1); j++)
					if (masks[i, j] == Mask.Prime)
						masks[i, j] = Mask.Empty;
		}
		private static void ClearCovers(bool[] rowsCovered, bool[] colsCovered)
		{// Method that writes false for boolean arrays
			if (rowsCovered.Length == colsCovered.Length)
				for (int i = 0; i < rowsCovered.Length; i++)
				{// If they are the same length
					rowsCovered[i] = false;
					colsCovered[i] = false;
				}
			else
			{// If their lengths are different
				for (int i = 0; i < rowsCovered.Length; i++)
					rowsCovered[i] = false;
				for (int i = 0; i < colsCovered.Length; i++)
					colsCovered[i] = false;
			}
		}
	}
}