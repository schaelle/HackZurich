using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json.Linq;

namespace HackZurich.Parser
{
	class Program
	{
		static void Main(string[] args)
		{
			ServicePointManager.DefaultConnectionLimit = 1000;

			AsyncMain().Wait();
		}

		public static async Task AsyncMain()
		{
			var parser = new FileParser();

			//var tmp = await (new AlternateTripProducer().ComputeAsync(new GeographicPosition(47.40205, 8.60959), new GeographicPosition(47.42455, 9.23813)));

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
			}

			IFirebaseConfig config = new FirebaseConfig
			{
				AuthSecret = "kpI2kVDB3WqSjSClhoK9XftXCpuuTvbu5BkEJPBA",
				BasePath = "https://hackzurich-1ced0.firebaseio.com/"
			};
			IFirebaseClient client = new FirebaseClient(config);

			var upload = true;

			foreach (var userTrip in userTrips)
			{
				if (upload)
					await client.DeleteAsync($"LogBox/{userTrip.Key}/Data");

				var uploadTasks = new List<Task>();
				for (int i = 0; i < userTrip.Value.Count; i++)
				{
					var trip = userTrip.Value[i];

					if (trip.Distance < 1)
						continue;

					//var path = Path.Combine(fieldPath, $"{new FileInfo(file).Name}_trip_{i}");
					//WriteRecords(path, trip.Records);

					Console.WriteLine($"Trip {i}: Distance: {trip.Distance:0.00}km, Fuel: {trip.Fuel:0.00}L, {trip.Fuel / trip.Distance * 100:0.00}L/100km");

					//var coordinates = Deduplicate(trip.Records.Where(a => a.Position != null).Select(a => a.Position));
					//var collection = new FeatureCollection();
					//collection.Features.Add(new Feature(new LineString(coordinates)));

					//var geopath = Path.Combine(fieldPath, $"{deviceId}_trip_{i}_geo");
					//using (var stream = new StreamWriter(new FileStream(geopath, FileMode.Create, FileAccess.Write)))
					//{
					//	_serializer.Serialize(stream, collection);
					//}

					var alternative = await new AlternateTripProducer().ComputeAsync(trip.StartPoint, trip.EndPoint);

					var tripData = new TripData
					{
						Distance = trip.Distance,
						DateTimeFrom = trip.Start,
						DateTimeTo = trip.End,
						Duration = (int)(trip.End - trip.Start).TotalSeconds,
						GasConsumption = trip.Fuel,
						StartPoint = trip.StartPoint,
						EndPoint = trip.EndPoint,
						Alternatives = alternative
						//Route = trip.Records.Where(a => a.Position != null).Select(a => new[] { a.Position.Latitude, a.Position.Longitude })
					};

					if (upload)
						uploadTasks.Add(client.SetAsync($"LogBox/{userTrip.Key}/Data/{i}", tripData));
				}
				await Task.WhenAll(uploadTasks);
			}

			//var values = await parser.ParseAsync(Path.Combine(path, Id));
			Console.WriteLine("Completed");
			Console.ReadLine();
		}

		private class TripData
		{
			public double Distance { get; set; }
			public DateTime DateTimeFrom { get; set; }
			public DateTime DateTimeTo { get; set; }
			public double GasConsumption { get; set; }
			public GeographicPosition StartPoint { get; set; }
			public GeographicPosition EndPoint { get; set; }
			public IReadOnlyList<AlternateTripProducer.TripDetail> Alternatives { get; set; }
			public int Duration { get; set; }

			//public IEnumerable<double[]> Route { get; set; }
		}
	}
}
