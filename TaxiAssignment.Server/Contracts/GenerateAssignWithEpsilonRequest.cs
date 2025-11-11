using System.ComponentModel.DataAnnotations;

namespace TaxiAssignment.Server.Contracts
{
	public record GenerateAssignWithEpsilonRequest(double[,] Distances, double EpsilonPrecision)
		: GenerateAssignRequest(Distances)
	{
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			foreach (var result in base.Validate(validationContext))
				yield return result;

			if (EpsilonPrecision < 0 || EpsilonPrecision > 1)
				yield return new ValidationResult(
					$"{nameof(EpsilonPrecision)} must be between 0 and 1.",
					[nameof(EpsilonPrecision)]
				);
		}
	}
}