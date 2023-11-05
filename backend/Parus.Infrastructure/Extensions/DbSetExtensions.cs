using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public static class DbSetExtensions
{
	/// <summary>
	/// Query the set for matches. All matching items are marked for deletion.
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	/// <param name="set"></param>
	/// <param name="match"></param>
	public static void RemoveWhere<TType>(this DbSet<TType> set, Expression<Func<TType, bool>> match) where TType : class
	{
		var results = set.Where(match);
		foreach (var result in results)
		{
			set.Remove(result);
		}
	}
}
