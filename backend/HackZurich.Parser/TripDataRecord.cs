using System;
using System.Collections.Generic;
using GeoJSON.Net.Geometry;

namespace HackZurich.Parser
{
	public class TripDataRecord
	{
		public DateTime Time { get; set; }
		public GeographicPosition Position { get; set; }
		public double? Speed { get; set; }
		public double? Fuel { get; set; }
		public double? Distance { get; set; }
	}

	public class TripFieldData
	{
		public DateTime Time { get; set; }
		public GeographicPosition Position { get; set; }
		public Dictionary<string, double> Values { get; set; } 
	}
}