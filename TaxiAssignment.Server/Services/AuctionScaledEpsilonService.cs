using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public class AuctionScaledEpsilonService : AuctionAssignmentService
	{
		private double? _min, _max;
		private double? _epsilon;

		protected override double[,] CreateSquareMatrix(AssignmentRequest request)
		{
			double[,] costs = request.Costs;

			int n = costs.GetLength(0), m = costs.GetLength(1);
			if (n == m)
				return costs;

			int maxLength = Math.Max(n, m);
			double[,] result = new double[maxLength, maxLength];
			_min = costs[0, 0];
			_max = costs[0, 0];

			for (int i = 0; i < costs.GetLength(0); i++)
				for (int j = 0; j < costs.GetLength(1); j++)
				{
					result[i, j] = costs[i, j];
					_min = Math.Min(_min.Value, costs[i, j]);
					_max = Math.Max(_max.Value, costs[i, j]);
				}

			double fillValue = request.FindMax ? _min.Value : _max.Value;

			if (costs.GetLength(0) > costs.GetLength(1))
			{// If there are more rows than columns
				for (int i = 0; i < costs.GetLength(0); i++)
					for (int j = costs.GetLength(1); j < costs.GetLength(0); j++)
						result[i, j] = fillValue;
			}
			else
			{// If there are more columns than rows
				for (int i = costs.GetLength(0); i < costs.GetLength(1); i++)
					for (int j = 0; j < costs.GetLength(1); j++)
						result[i, j] = fillValue;
			}

			return result;
		}
		protected override double CalculateEpsilon(double[,] costs, int n, double? epsilonPrecision)
		{
			const double STEP = 5;
			const double DEFAULT_EPSILON_PRECISION = 0.5;

			epsilonPrecision ??= DEFAULT_EPSILON_PRECISION;

			if (!_epsilon.HasValue)
			{
				if (!_min.HasValue || !_max.HasValue)
				{
					_min = costs[0, 0];
					_max = costs[0, 0];

					for (int i = 0; i < costs.GetLength(0); i++)
						for (int j = 0; j < costs.GetLength(1); j++)
						{
							_min = Math.Min(_min.Value, costs[i, j]);
							_max = Math.Max(_max.Value, costs[i, j]);
						}
				}
				_epsilon = (_max.Value - _min.Value) / n;
			}
			else if (_min.HasValue && _max.HasValue)
			{
				double epsilonMin = 1.0 / n;
				double epsilonMax = (_max.Value - _min.Value) / n;

				double epsilonThreshold = (1 - epsilonPrecision.Value) * epsilonMax
					+ epsilonPrecision.Value * epsilonMin;

				if (_epsilon.Value > epsilonThreshold)
					_epsilon /= STEP;
			}

			return _epsilon.Value;
		}

		protected override void ResetAuctionState()
		{
			_min = null;
			_max = null;
			_epsilon = null;
		}
	}
}