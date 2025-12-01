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

		private Mask[,] _masks = new Mask[0, 0];
		private bool[] _rowsCovered = [], _colsCovered = [];
		private int _height, _width;

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

			_height = costs.GetLength(0);
			_width = costs.GetLength(1);

			if (request.FindMax)
				InvertCosts(costs);

			ReduceRows(costs);
			ReduceColumns(costs);
			InitializeMasks(costs);

			RunAlgorithm(costs);

			_masks = CopyMaskMatrix(n, m);
			return GenerateAgentsTasks();
		}

		private static double[,] CreateSquareMatrix(double[,] costs)
		{
			int n = costs.GetLength(0), m = costs.GetLength(1);
			if (n == m)
				return costs;

			int maxLength = Math.Max(n, m);
			double[,] result = new double[maxLength, maxLength];
			double maxValue = costs[0, 0];

			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
				{
					result[i, j] = costs[i, j];
					if (costs[i, j] > maxValue)
						maxValue = costs[i, j];
				}

			IAssignmentService.FillExtraCells(result, n, m, maxValue);

			return result;
		}
		private void InvertCosts(double[,] costs)
		{
			for (int i = 0; i < _height; i++)
			{
				double max = costs[i, 0];
				for (int j = 0; j < _width; j++)
					if (costs[i, j] > max)
						max = costs[i, j];

				for (int j = 0; j < _width; j++)
					costs[i, j] = max - costs[i, j];
			}
		}
		private void ReduceRows(double[,] costs)
		{
			for (int i = 0; i < _height; i++)
			{
				double min = costs[i, 0];// Minimum value in the current row
				for (int j = 0; j < _width; j++)
					if (costs[i, j] < min)
						min = costs[i, j];

				for (int j = 0; j < _width; j++)
					costs[i, j] -= min;
			}
		}
		private void ReduceColumns(double[,] costs)
		{
			for (int i = 0; i < _width; i++)
			{
				double min = costs[0, i];// Minimum value in the current column
				for (int j = 0; j < _height; j++)
					if (costs[j, i] < min)
						min = costs[j, i];

				for (int j = 0; j < _height; j++)
					costs[j, i] -= min;
			}
		}
		private void InitializeMasks(double[,] costs)
		{
			_masks = new Mask[_height, _width];
			_rowsCovered = new bool[_height];
			_colsCovered = new bool[_width];

			for (int i = 0; i < _height; i++)
				for (int j = 0; j < _width; j++)
					if (Math.Abs(costs[i, j]) < EPSILON && !_rowsCovered[i] && !_colsCovered[j])
					{
						_masks[i, j] = Mask.Star;
						_rowsCovered[i] = true;
						_colsCovered[j] = true;
					}

			ClearCovers();
		}

		private void RunAlgorithm(double[,] costs)
		{
			Location[] path = new Location[_height * _width];
			Location pathStart = default;
			HungarianStep step = HungarianStep.MarkStars;

			while (step != HungarianStep.Last)
			{
				if (step == HungarianStep.MarkStars)
					step = MarkInitialStars();
				if (step == HungarianStep.PrimeAndCover)
					step = PrimeAndCover(costs, ref pathStart);
				if (step == HungarianStep.BuildPath)
					step = BuildAndConvertPath(path, pathStart);
				if (step == HungarianStep.AdjustCosts)
					step = AdjustCosts(costs);
			}
		}

		private HungarianStep MarkInitialStars()
		{
			for (int i = 0; i < _masks.GetLength(0); i++)
				for (int j = 0; j < _masks.GetLength(1); j++)
					if (_masks[i, j] == Mask.Star)
						_colsCovered[j] = true;

			int colsCoveredCount = 0;
			for (int j = 0; j < _colsCovered.Length; j++)
				if (_colsCovered[j])
					colsCoveredCount++;

			// If all columns are crossed out, this is the last step in the cycle.
			if (colsCoveredCount == _colsCovered.Length)
				return HungarianStep.Last;

			return HungarianStep.PrimeAndCover;
		}
		private HungarianStep PrimeAndCover(double[,] costs, ref Location pathStart)
		{
			while (true)
			{
				Location location = FindZero(costs);
				if (location.Row == EMPTY_ROW_INDEX)// If no uncrossed zero was found
					return HungarianStep.AdjustCosts;

				_masks[location.Row, location.Column] = Mask.Prime;

				int indexCol = FindColumnInRow(location.Row, Mask.Star);
				if (indexCol != EMPTY_COLUMN_INDEX)
				{
					_rowsCovered[location.Row] = true;
					_colsCovered[indexCol] = false;
				}
				else
				{
					pathStart = location;
					return HungarianStep.BuildPath;
				}
			}
		}
		private HungarianStep BuildAndConvertPath(Location[] path, Location pathStart)
		{
			int pathIndex = 0;
			path[0] = pathStart;

			while (true)
			{
				int row = FindRowInColumn(path[pathIndex].Column, Mask.Star);
				if (row == EMPTY_ROW_INDEX)
					break;

				pathIndex++;
				path[pathIndex] = new Location(row, path[pathIndex - 1].Column);

				int col = FindColumnInRow(path[pathIndex].Row, Mask.Prime);

				pathIndex++;
				path[pathIndex] = new Location(path[pathIndex - 1].Row, col);
			}

			ConvertPath(path, pathIndex + 1);
			ClearCovers();
			ClearPrimes();

			return HungarianStep.MarkStars;
		}
		private HungarianStep AdjustCosts(double[,] costs)
		{
			double minValue = FindMinimum(costs);

			// Add or subtract the minimum depending on the strikethrough
			for (int i = 0; i < costs.GetLength(0); i++)
				for (int j = 0; j < costs.GetLength(1); j++)
				{
					if (_rowsCovered[i])
						costs[i, j] += minValue;
					if (!_colsCovered[j])
						costs[i, j] -= minValue;
				}

			return HungarianStep.PrimeAndCover;
		}

		private double FindMinimum(double[,] costs)
		{// Method that finds the minimum value among NOT crossed out elements
			double min = double.MaxValue;

			for (int i = 0; i < costs.GetLength(0); i++)
				for (int j = 0; j < costs.GetLength(1); j++)
					if (!_rowsCovered[i] && !_colsCovered[j] && costs[i, j] < min)
						min = costs[i, j];

			return min;
		}
		private int FindColumnInRow(int rowIndex, Mask maskToFind)
		{
			for (int j = 0; j < _masks.GetLength(1); j++)
				if (_masks[rowIndex, j] == maskToFind)
					return j;

			return EMPTY_COLUMN_INDEX;
		}
		private int FindRowInColumn(int columnIndex, Mask maskToFind)
		{
			for (int i = 0; i < _masks.GetLength(0); i++)
				if (_masks[i, columnIndex] == maskToFind)
					return i;

			return EMPTY_ROW_INDEX;
		}

		private Location FindZero(double[,] costs)
		{// Method returns the position of the first uncrossed zero
			for (int i = 0; i < costs.GetLength(0); i++)
				for (int j = 0; j < costs.GetLength(1); j++)
					if (Math.Abs(costs[i, j]) < EPSILON && !_rowsCovered[i] && !_colsCovered[j])
						return new Location(i, j);

			return new Location(EMPTY_ROW_INDEX, EMPTY_COLUMN_INDEX);
		}

		private void ConvertPath(Location[] path, int pathLength)
		{
			int row, column;

			for (int i = 0; i < pathLength; i++)
			{
				row = path[i].Row;
				column = path[i].Column;

				if (_masks[row, column] == Mask.Star)
					_masks[row, column] = Mask.Empty;

				else if (_masks[row, column] == Mask.Prime)
					_masks[row, column] = Mask.Star;
			}
		}
		private void ClearPrimes()
		{
			for (var i = 0; i < _masks.GetLength(0); i++)
				for (var j = 0; j < _masks.GetLength(1); j++)
					if (_masks[i, j] == Mask.Prime)
						_masks[i, j] = Mask.Empty;
		}
		private void ClearCovers()
		{
			Array.Clear(_rowsCovered);
			Array.Clear(_colsCovered);
		}

		private Mask[,] CopyMaskMatrix(int height, int width)
		{
			Mask[,] newMasks = new Mask[height, width];
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++)
					newMasks[i, j] = _masks[i, j];

			return newMasks;
		}
		private int[] GenerateAgentsTasks()
		{// Method returns resulting array, where the index is the worker and the value is the task
			const int UNKNOWN_TASK = -1;
			int[] agentsTasks = new int[_masks.GetLength(0)];

			for (int i = 0; i < _masks.GetLength(0); i++)
				for (int j = 0; j < _masks.GetLength(1); j++)
				{
					if (_masks[i, j] == Mask.Star)
					{
						agentsTasks[i] = j;
						break;
					}
					else if (j == _masks.GetLength(1) - 1)
						agentsTasks[i] = UNKNOWN_TASK;
				}

			return agentsTasks;
		}
	}
}