using System.Text.Json;

namespace GatherSpot.API.Extensions
{
	public static class HttpExtensions
	{
		public static void AddPaginationHeader(this HttpResponse response, 
			int currentPage, int itemsPerPage, int totalItems, int totalPages) 
		{
			var paginationHeader = new
			{
				currentPage,
				totalPages,
				itemsPerPage,
				totalItems
			};
			response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
			response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
			// we will return the pagination information as a Http Response Header,
			// and since it's a custom header we need to expose it so that the client can see it
		}
	}
}
