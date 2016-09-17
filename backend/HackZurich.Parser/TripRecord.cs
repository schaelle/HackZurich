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
				StartDistance = distanceRecords.Min(i => i.Distance.Value);
				EndDistance = distanceRecords.Max(i => i.Distance.Value);
				Distance = distanceRecords.Max(i => i.Distance.Value) - distanceRecords.Min(i => i.Distance.Value);
			}
		}

		
		public DateTime Start => Records.Min(i => i.Time);
		public DateTime End => Records.Max(i => i.Time);

		public double StartDistance { get; }
		public double EndDistance { get; }

		public double Distance { get; }
		public double Fuel { get; set; }

		public IReadOnlyList<TripDataRecord> Records { get; }
	}
}