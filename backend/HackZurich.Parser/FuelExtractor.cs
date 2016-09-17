using System.Collections.Generic;
using System.Linq;

namespace HackZurich.Parser
{
	public class FuelExtractor : IExtractor<TripDataRecord, TripDataRecord>
	{
		public IReadOnlyList<TripDataRecord> Extract(IEnumerable<TripDataRecord> records)
		{
			return records.Where(i => i.Fuel.HasValue).ToList();
		}
	}
}