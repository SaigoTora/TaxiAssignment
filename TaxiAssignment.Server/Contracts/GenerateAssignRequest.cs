using System.ComponentModel.DataAnnotations;

namespace TaxiAssignment.Server.Contracts
{
	public record GenerateAssignRequest(double[,] Distances) : IValidatableObject
	{
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (Distances == null || Distances.GetLength(0) == 0 || Distances.GetLength(1) == 0)
				yield return new ValidationResult("Distances must have at least one row " +
					"and one column.", [nameof(Distances)]);
		}
	}
}