using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HackZurich.Parser
{
	public class Record
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("id_str")]
		public string IdStr { get; set; }

		[JsonProperty("asset")]
		public string Asset { get; set; }

		[JsonProperty("recorded_at_ms")]
		public DateTime RecorderdAt { get; set; }

		[JsonProperty("loc")]
		public double[] Location { get; set; }

		[JsonProperty("index")]
		public int Index { get; set; }

		public Dictionary<string, Base64Value> Fields { get; set; }

		public double? FieldValue(string field)
		{
			Base64Value value;
			if (Fields.TryGetValue(field, out value))
				return value.AsInt();

			return null;
		}
	}
}