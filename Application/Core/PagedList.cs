using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
	public class PagedList<T> : List<T> // purpose of this class, is to return a paginated list with all pagination properties
	{
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }

		public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
		{
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			PageSize = pageSize;
			TotalCount = count;
			AddRange(items);
		}

		public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
		{
			// IQueryable is a prepared query that is not executed until you loop, use ToList/ToArray
			// or use aggregate function like count
			var count = await source.CountAsync();
			var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
			return new PagedList<T>(items, count, pageNumber, pageSize);
		}
	}
}
