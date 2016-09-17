using System;
using System.Collections.Generic;

namespace HackZurich.Parser
{
	public class TripExtractor : IExtractor<TripDataRecord, TripRecord>
	{
		private static readonly TimeSpan Deadtime = TimeSpan.FromMinutes(2);

		public IReadOnlyList<TripRecord> Extract(IEnumerable<TripDataRecord> records)
		{
			var lastMoving = DateTime.MinValue;

			var result = new List<TripRecord>();
			var res = new List<TripDataRecord>();
			foreach (var record in records)
			{
				if (record.Speed > 0)
				{
					if (record.Time - lastMoving > Deadtime)
					{
						if (res.Count > 0)
							result.Add(new TripRecord(res));
						res.Clear();
					}

					res.Add(record);
					lastMoving = record.Time;
				}
			}

			return result;
		}
	}
}