using TaxiAssignment.Server.Interfaces;
using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Services
{
	public abstract class AuctionAssignmentService : IAssignmentService
	{
		private readonly record struct BestTasksResult(int BestTask, double BestValue,
			double SecondBestValue);

		private const int UNKNOWN_TASK = -1;
		private const int UNKNOWN_BEST_TASK = -1;

		private int _minDimension;
		private bool? _hasMoreRows;

		private int[] _agentsTasks = [];
		private double[] _prices = [];

		public int[] Solve(AssignmentRequest request)
		{
			ArgumentNullException.ThrowIfNull(request);
			ArgumentNullException.ThrowIfNull(request.Costs);

			ResetAuctionState();

			double[,] costs = PrepareCostMatrix(request);
			int n = costs.GetLength(0);
			InitializeAuctionArrays(n);

			double? epsilonPrecision = null;
			if (request is AuctionScaledRequest scaledRequest)
				epsilonPrecision = scaledRequest.EpsilonPrecision;

			RunAlgorithm(costs, request.FindMax, epsilonPrecision);

			return GenerateAgentsTasks();
		}
		private double[,] PrepareCostMatrix(AssignmentRequest request)
		{
			double[,] costs = request.Costs;

			int n = costs.GetLength(0), m = costs.GetLength(1);
			_hasMoreRows = null;
			if (n > m)
			{
				_minDimension = m;
				_hasMoreRows = true;
			}
			else if (n < m)
			{
				_minDimension = n;
				_hasMoreRows = false;
			}
			else
				_minDimension = n;

			if (n != m)
				costs = CreateSquareMatrix(request);

			return costs;
		}
		private void InitializeAuctionArrays(int n)
		{
			_agentsTasks = new int[n];
			_prices = new double[n];

			for (int i = 0; i < n; i++)
				_agentsTasks[i] = UNKNOWN_TASK;
		}

		protected abstract double[,] CreateSquareMatrix(AssignmentRequest request);
		protected abstract double CalculateEpsilon(double[,] costs, int n,
			double? epsilonPrecision);
		protected virtual void ResetAuctionState() { }

		private void RunAlgorithm(double[,] costs, bool findMax, double? epsilonPrecision)
		{
			bool unassignedExists;
			int n = costs.GetLength(0);

			do
			{
				unassignedExists = false;
				double epsilon = CalculateEpsilon(costs, n, epsilonPrecision);
				for (int agent = 0; agent < n; agent++)
				{
					if (_agentsTasks[agent] != UNKNOWN_TASK)
						continue;

					unassignedExists = true;
					BestTasksResult bestTasks = GetBestTasks(costs, n, agent, findMax);

					if (bestTasks.BestTask != UNKNOWN_BEST_TASK)
					{
						double bid = bestTasks.BestValue - bestTasks.SecondBestValue + epsilon;
						UpdatePricesAndAssignments(n, agent, bestTasks.BestTask, bid);
					}
				}
			} while (unassignedExists);
		}
		private BestTasksResult GetBestTasks(double[,] costs, int n, int agent, bool findMax)
		{
			int bestTask = UNKNOWN_BEST_TASK;
			double bestValue = double.NegativeInfinity, secondBestValue = double.NegativeInfinity;

			for (int task = 0; task < n; task++)
			{
				double value = (findMax ? costs[agent, task] : -costs[agent, task])
					- _prices[task];
				if (value > bestValue)
				{
					secondBestValue = bestValue;
					bestValue = value;
					bestTask = task;
				}
				else if (value > secondBestValue)
					secondBestValue = value;
			}

			return new(bestTask, bestValue, secondBestValue);
		}
		private void UpdatePricesAndAssignments(int n, int agent, int bestTask, double bid)
		{
			_prices[bestTask] += bid;

			for (int otherAgent = 0; otherAgent < n; otherAgent++)
				if (_agentsTasks[otherAgent] == bestTask)
				{
					_agentsTasks[otherAgent] = UNKNOWN_TASK;
					break;
				}

			_agentsTasks[agent] = bestTask;
		}
		private int[] GenerateAgentsTasks()
		{
			if (_hasMoreRows.HasValue)
			{
				if (_hasMoreRows.Value)
				{
					for (int i = 0; i < _agentsTasks.Length; i++)
						if (_agentsTasks[i] >= _minDimension)
							_agentsTasks[i] = UNKNOWN_TASK;
				}
				else
					return [.. _agentsTasks.Take(_minDimension)];
			}

			return _agentsTasks;
		}
	}
}