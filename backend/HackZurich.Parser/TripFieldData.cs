using System;
using System.Collections.Generic;
using GeoJSON.Net.Geometry;

namespace HackZurich.Parser
{
	public class TripFieldData
	{
		public DateTime Time { get; set; }
		public GeographicPosition Position { get; set; }
		public Dictionary<string, double> Values { get; set; } 
	}
}