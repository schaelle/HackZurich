using System.Collections.Generic;
using GeoJSON.Net.Geometry;

namespace HackZurich.Parser
{
	public class FieldExtractor : IExtractor<Record, TripFieldData>
	{
		private readonly string[] _fields;

		public FieldExtractor(string[] fields)
		{
			_fields = fields;
		}

		public IReadOnlyList<TripFieldData> Extract(IEnumerable<Record> records)
		{
			var res = new List<TripFieldData>();
			foreach (var record in records)
			{
				var data = new Dictionary<string, double>();
				for (var i = 0; i < _fields.Length; i++)
				{
					var field = _fields[i];

					var value = record.FieldValue(field);
					if (value.HasValue)
						data[field] = value.Value;
				}

				res.Add(new TripFieldData
				{
					Time = record.RecorderdAt,
					Position = record.Location != null ? new GeographicPosition(record.Location[1], record.Location[1]) : null,
					Values = data
				});
			}

			return res;
		}
	}
}