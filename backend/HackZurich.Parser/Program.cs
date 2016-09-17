using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GeoJSON.Net.Feature;
using Newtonsoft.Json.Linq;

namespace HackZurich.Parser
{
	class Program
	{
		//private const string Id = "356156068026087_porscheinte_2016-09-04T14_53_43Z_2016-09-05T15_17_41Z";
		private const string Id = "356156068026087_porscheinte_2016-09-12T15_26_04Z_2016-09-13T16_01_32Z";

		static void Main(string[] args)
		{
			AsyncMain().Wait();
		}

		public static async Task AsyncMain()
		{
			var parser = new FileParser();

			const string path = @"D:\Projects\HackZurich\backend\rawData\temp\json";

			var tasks = new List<Task<IEnumerable<TripDataRecord>>>();
			foreach (var file in Directory.GetFiles(path))
			{
				tasks.Add(parser.ParseAsync(file));
			}
			var result = (await Task.WhenAll(tasks)).SelectMany(i => i).ToList();

			var fuelTime = new Dictionary<string, ProximityDatabase<DateTime>>();
			var fuelDistance = new Dictionary<string, ProximityDatabase<double>>();
			foreach (var grouping in result.Where(i => i.Fuel.HasValue).GroupBy(i => i.DeviceId))
			{
				var fuelDb = new ProximityDatabase<DateTime>(grouping.Select(i => new Tuple<DateTime, double>(i.Time, i.Fuel.Value)), (t1, t2) => (t1 - t2).TotalSeconds, 120);
				fuelTime.Add(grouping.Key, fuelDb);

				var fuelDistanceDb = new ProximityDatabase<double>(grouping.Where(i => i.Distance.HasValue).Select(i => new Tuple<double, double>(i.Distance.Value, i.Fuel.Value)), (t1, t2) => t1 - t2, 2);
				fuelDistance.Add(grouping.Key, fuelDistanceDb);
			}

			var userTrips = new Dictionary<string, List<TripRecord>>();
			foreach (var grouping in result.GroupBy(i => i.DeviceId))
			{
				var localTrips = new List<TripRecord>();

				var trips = new TripExtractor().Extract(grouping);
				foreach (var trip in trips)
				{
					if (trip.Distance > 10)
					{
						localTrips.Add(trip);

						var startDistance = trip.Records.First(i => i.Distance.HasValue).Distance.Value;
						var endDistance = trip.Records.Last(i => i.Distance.HasValue).Distance.Value;

						double startfuel;
						double endfuel;

						if (fuelDistance[grouping.Key].TryInterpolate(startDistance, out startfuel) &&
							fuelDistance[grouping.Key].TryInterpolate(endDistance, out endfuel))
						{
							trip.Fuel = endfuel - startfuel;
						}

						Console.WriteLine($"{trip.Fuel:0.00}L, {trip.Distance:0.00}km, {trip.Fuel / trip.Distance * 100:0.00}L/100km");
					}
				}

				userTrips.Add(grouping.Key, localTrips);

				//var fuelDb = new ProximityDatabase(grouping.Select(i => new Tuple<DateTime, double>(i.Time, i.Fuel.Value)));
				//fuel.Add(grouping.Key, fuelDb);
			}

			//var values = await parser.ParseAsync(Path.Combine(path, Id));
			Console.WriteLine("Completed");
			Console.ReadLine();
		}
	}
}
