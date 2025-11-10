using System.ComponentModel.DataAnnotations;

namespace TaxiAssignment.Server.Contracts
{
	public record GenerateAssignRequest(double[,] Distances, double? EpsilonPrecision)
		: IValidatableObject
	{
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (Distances == null || Distances.GetLength(0) == 0 || Distances.GetLength(1) == 0)
				yield return new ValidationResult($"{nameof(Distances)} must have at least " +
					$"one row and one column.", [nameof(Distances)]);

			if (EpsilonPrecision.HasValue && (EpsilonPrecision < 0 || EpsilonPrecision > 1))
				yield return new ValidationResult(
					$"{nameof(EpsilonPrecision)} must be between 0 and 1.",
					[nameof(EpsilonPrecision)]
				);
		}
	}
}