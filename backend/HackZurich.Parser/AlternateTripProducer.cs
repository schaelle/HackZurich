using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GeoJSON.Net.Geometry;

namespace HackZurich.Parser
{
	public class AlternateTripProducer
	{
		private const string Key = "AIzaSyDzO8mRAp15BNGX4N_7fLNqn81170kCHLM";

		private static readonly Dictionary<string, double> Co2Mode = new Dictionary<string, double>
		{
			["driving"] = 150,
			["bicycling"] = 0,
			["transit"] = 40,
			["walking"] = 0,
		};

		public async Task<IReadOnlyList<TripDetail>> ComputeAsync(GeographicPosition start, GeographicPosition stop)
		{
			return await Task.WhenAll(Co2Mode.Keys.Select(mode => LoadAsync(start, stop, mode)));
		}

		private static async Task<TripDetail> LoadAsync(GeographicPosition start, GeographicPosition stop, string mode)
		{
			var client = new HttpClient();
			var response = await client.GetAsync($"https://maps.googleapis.com/maps/api/directions/json?origin={start.Latitude} {start.Longitude}&destination={stop.Latitude} {stop.Longitude}&key={Key}&mode={mode}");
			var way = await response.Content.ReadAsAsync<Model>();

			var minDuration = double.MaxValue;
			Leg min = null;

			foreach (var route in way.Routes)
			{
				foreach (var leg in route.Legs)
				{
					if (leg.Duration.Value < minDuration)
					{
						minDuration = leg.Duration.Value;
						min = leg;
					}
				}
			}

			if (min == null)
				return null;

			return new TripDetail
			{
				Mode = mode,
				Duration = min.Duration.Value,
				Distance = min.Distance.Value,
				Co2 = min.Distance.Value / 1000 * Co2Mode[mode]
			};
		}

		public class TripDetail
		{
			public string Mode { get; set; }
			public double Duration { get; set; }
			public double Distance { get; set; }
			public double Co2 { get; set; }
		}

		private class Model
		{
			public IEnumerable<Route> Routes { get; set; }
		}

		public class Route
		{
			public IEnumerable<Leg> Legs { get; set; }
		}

		public class Leg
		{
			public TextDoubleValue Duration { get; set; }
			public TextDoubleValue Distance { get; set; }
		}

		public class TextDoubleValue
		{
			public string Text { get; set; }
			public double Value { get; set; }
		}
	}
}