using System.Diagnostics.CodeAnalysis;

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
			SetMinMax(costs, (i, j) => result[i, j] = costs[i, j]);

			double fillValue = request.FindMax ? _min.Value : _max.Value;
			if (n > m)
			{// If there are more rows than columns
				for (int i = 0; i < n; i++)
					for (int j = m; j < n; j++)
						result[i, j] = fillValue;
			}
			else
			{// If there are more columns than rows
				for (int i = n; i < m; i++)
					for (int j = 0; j < m; j++)
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
					SetMinMax(costs);

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

		[MemberNotNull(nameof(_min), nameof(_max))]
		private void SetMinMax(double[,] costs, Action<int, int>? onCell = null)
		{
			int n = costs.GetLength(0), m = costs.GetLength(1);
			_min = costs[0, 0];
			_max = costs[0, 0];

			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
				{
					onCell?.Invoke(i, j);
					if (costs[i, j] < _min.Value)
						_min = costs[i, j];
					if (costs[i, j] > _max.Value)
						_max = costs[i, j];
				}
		}
	}
}