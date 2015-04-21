using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using NarenT.Common;

namespace NarenT.HTTP
{
	public class RoutingEngine : IEnumerable
	{
		private readonly List<Route> Routes = new List<Route> ();

		public RoutingEngine ()
		{
		}

		public IOption<Route> Resolve(string path)
		{
			return this.Routes.FirstOrDefault(r => r.MatchPredicate(path)).SomeOrNone();
		}

		public void AddRoute(Func<IHttpAction> actionFactory, Func<string, bool> matchPredicate)
		{
			this.Routes.Add(new Route(actionFactory, matchPredicate));
		}

		#region IEnumerable implementation

		public IEnumerator GetEnumerator ()
		{
			foreach (var r in this.Routes)
				yield return r;
		}

		#endregion

		public int Add (IEnumerable<Route> routes)
		{
			return AddAll (routes);
		}

		public int AddAll (IEnumerable<Route> routes)
		{
			int count = 0;
			foreach (var r in routes){
				this.Routes.Add(r);
				count++;
			}
			return count;
		}
	}

	public class Route
	{
		public Func<IHttpAction> ActionFactory {
			get;
			set;
		}

		public Func<string, bool> MatchPredicate {
			get;
			set;
		}

		public Route(Func<IHttpAction> actionFactory, Func<string, bool> matchPredicate)
		{
			this.ActionFactory = actionFactory;
			this.MatchPredicate = matchPredicate;
		}
	}
}
