using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Interfaces
{
	public interface IRoadPointsService
	{
		/// <summary>
		/// Returns a specified number of random road points for a given city.
		/// </summary>
		/// <param name="city">The city for which to generate random road points.</param>
		/// <param name="count">The number of random points to return.</param>
		/// <returns>An array of Location objects representing random points on roads in the specified city.</returns>
		Location[] GetRandomPointsForCity(City city, int count);
	}
}