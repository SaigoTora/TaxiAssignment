using TaxiAssignment.Server.Models;

namespace TaxiAssignment.Server.Interfaces
{
	public interface IGeoPointsService
	{
		/// <summary>
		/// Returns a specified number of random road points for a given city.
		/// </summary>
		/// <param name="city">The city for which to generate random road points.</param>
		/// <param name="count">The number of random points to return.</param>
		/// <returns>An array of Location objects representing random points on roads in the specified city.</returns>
		Location[] GetRandomPointsOnRoad(City city, int count);

		/// <summary>
		/// Returns a specified number of random points anywhere within the boundaries of the given city.
		/// The points are not restricted to roads.
		/// </summary>
		/// <param name="city">The city for which to generate random points.</param>
		/// <param name="count">The number of random points to return.</param>
		/// <returns>An array of Location objects representing random points anywhere within the city.</returns>
		Location[] GetRandomPointsInCity(City city, int count);
	}
}