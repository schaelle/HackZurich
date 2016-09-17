using System;
using System.Collections.Generic;
using System.Linq;

namespace HackZurich.Parser
{
	public class TripRecord
	{
		public TripRecord(IReadOnlyList<TripDataRecord> records)
		{
			Records = records.ToList();

			var distanceRecords = Records.Where(i => i.Distance.HasValue).ToList();
			if (distanceRecords.Count > 0)
			{
				var minDistance = distanceRecords.Min(i => i.Distance.Value);
				var maxDistance = distanceRecords.Max(i => i.Distance.Value);
				Distance = maxDistance - minDistance;
			}

			var fuelRecords = Records.Where(i => i.Fuel.HasValue).ToList();
			if (fuelRecords.Count > 0)
			{
				var min = fuelRecords.Min(i => i.Fuel.Value);
				var max = fuelRecords.Max(i => i.Fuel.Value);
				Fuel = max - min;
			}
		}

		
		public DateTime Start => Records.Min(i => i.Time);
		public DateTime End => Records.Max(i => i.Time);

		public double Distance { get; }
		public double Fuel { get; }

		public IReadOnlyList<TripDataRecord> Records { get; }
	}
}