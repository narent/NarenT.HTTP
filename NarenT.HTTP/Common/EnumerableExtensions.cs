using System;
using System.Collections.Generic;
using System.Linq;

namespace NarenT.Common
{
    public static class EnumerableExtensions
    {
        public static T FirstOrLazy<T>(this IEnumerable<T> source, Func<T> lazy)
        {
            var list = source.Take(1).ToList();
            return list.Any() ? list.First() : lazy();
        }

        public static IEnumerable<TResult> Aggregates<T, TResult>(this IEnumerable<T> enumerable, TResult seed, Func<TResult, T, TResult> func)
        {
            TResult[] accum = { seed };
            foreach (var sumEach in enumerable.Select(item => func(accum[0], item)))
            {
                accum[0] = sumEach;
                yield return sumEach;
            }
        }

		public static T FirstOr<T>(this IEnumerable<T> someEnum, T def) where T : class
		{
			var firstOrDefault = someEnum.FirstOrDefault();
			return firstOrDefault ?? def;
		}
    }
}