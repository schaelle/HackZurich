using System;
using System.Collections.Generic;
using System.Linq;

namespace HackZurich.Parser
{
	public class ProximityDatabase<TKey> where TKey : IComparable<TKey>
	{
		private readonly IEnumerable<Tuple<TKey, double>> _values;
		private readonly Func<TKey, TKey, double> _distance;
		private readonly double _maxDistance;

		public ProximityDatabase(IEnumerable<Tuple<TKey, double>> values, Func<TKey, TKey, double> distance, double maxDistance)
		{
			_values = values;
			_distance = distance;
			_maxDistance = maxDistance;
		}

		public bool TryLookup(TKey key, out double value)
		{
			var minDist = double.MaxValue;
			var closest = default(TKey);

			foreach (var tuple in _values)
			{
				var dist = Math.Abs(_distance(tuple.Item1, key));
				if (dist < minDist)
				{
					closest = tuple.Item1;
					minDist = dist;
				}
			}

			if (default(TKey).Equals(closest) || _maxDistance < minDist)
			{
				value = 0;
				return false;
			}

			value = _values.First(i => i.Item1.Equals(closest)).Item2;
			return true;
		}

		public bool TryInterpolate(TKey key, out double value)
		{
			var minDistLower = double.MinValue;
			var minDistUpper = double.MaxValue;
			var closestLower = default(TKey);
			var closestUpper = default(TKey);
			double valueLower = 0;
			double valueUpper = 0;

			foreach (var tuple in _values)
			{
				var dist = _distance(tuple.Item1, key);
				if (dist < 0)
				{
					if (dist > minDistLower)
					{
						closestLower = tuple.Item1;
						valueLower = tuple.Item2;
						minDistLower = dist;
					}
				}
				else
				{
					if (dist < minDistUpper)
					{
						closestUpper = tuple.Item1;
						valueUpper = tuple.Item2;
						minDistUpper = dist;
					}
				}
			}

			if (_distance(closestUpper, key) == 0)
				value = valueUpper;
			else
				value = (valueUpper - valueLower) / _distance(closestUpper, closestLower) * _distance(key, closestLower) + valueLower;

			return true;
		}
	}
}