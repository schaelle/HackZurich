using System.Collections.Generic;
using GeoJSON.Net.Geometry;

namespace HackZurich.Parser
{
	public class FieldSpecificExtractor : IExtractor<Record, TripDataRecord>
	{
		private const string SpeedField = "GPS_SPEED";
		private const string FullField = "MDI_OBD_FUEL";
		private const string DistanceField = "ODO_FULL_METER";

		public IReadOnlyList<TripDataRecord> Extract(IEnumerable<Record> records)
		{
			var res = new List<TripDataRecord>();
			foreach (var record in records)
			{
				if (record.Location == null)
					continue;

				double speed = 0;
				var speedValue = record.FieldValue(SpeedField);
				if (speedValue.HasValue)
					speed = speedValue.Value * 1.85 / 1000;

				var fullValue = record.FieldValue(FullField);
				var distanceValue = record.FieldValue(DistanceField);

				res.Add(new TripDataRecord
				{
					DeviceId = record.Asset,
					Time = record.RecorderdAt,
					Position = new GeographicPosition(record.Location[1], record.Location[0]),
					Speed = speed,
					Fuel = fullValue / 1000,
					Distance = distanceValue / 1000
				});
			}
			return res;
		}

	}
}