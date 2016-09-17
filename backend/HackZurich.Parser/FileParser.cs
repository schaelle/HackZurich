using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;

namespace HackZurich.Parser
{
	public class FileParser
	{
		private readonly JsonSerializer _serializer = new JsonSerializer();

		public async Task<IEnumerable<TripDataRecord>> ParseAsync(string file)
		{
			var reader = new StreamReader(new FileStream(file, FileMode.Open));

			var result = new List<Record>();
			while (!reader.EndOfStream)
			{
				var line = await reader.ReadLineAsync();
				var record = _serializer.Deserialize<Record>(new JsonTextReader(new StringReader(line)));
				result.Add(record);
			}
			result = result.OrderBy(i => i.RecorderdAt).ToList();

			var deviceId = result.Select(i => i.Id).First();
			Console.WriteLine($"{deviceId} #{result.Count}");

			//var paths = new PathExtractor().Extract(result).ToList();
			//if (paths.Count >= 4)
			//{
			//	var coordinates = Deduplicate(paths.Select(i => new GeographicPosition(i.Item2[1], i.Item2[0])));

			//	var collection = new FeatureCollection();
			//	collection.Features.Add(new Feature(new LineString(coordinates)));

			//	var path = @"D:\Projects\HackZurich\backend\rawData\temp\path";
			//	using (var stream = new StreamWriter(new FileStream(Path.Combine(path, new FileInfo(file).Name), FileMode.Create, FileAccess.Write)))
			//	{
			//		_serializer.Serialize(stream, collection);
			//	}
			//}
			var fieldPath = @"D:\Projects\HackZurich\backend\rawData\temp\field";
			var fieldsNames = new FieldNameExtractor().Extract(result).ToArray();
			//using (var writer = new StreamWriter(new FileStream(Path.Combine(fieldPath, "fields"), FileMode.Create, FileAccess.Write)))
			//{
			//	foreach (var fieldsName in fieldsNames)
			//	{
			//		writer.WriteLine(fieldsName);
			//	}
			//}

			//fieldsNames = new[] { "GPS_SPEED", "ODO_FULL_METER", "MDI_OBD_RPM", "MDI_JOURNEY_TIME", "MDI_OBD_MILEAGE", "MDI_OBD_FUEL", "ENH_DASHBOARD_FUEL" };
			//var fieldRawValue = new FieldExtractor(fieldsNames).Extract(result).ToList();
			//var rawPath = Path.Combine(fieldPath, deviceId);
			//using (var writer = new StreamWriter(new FileStream(rawPath, FileMode.Create, FileAccess.Write)))
			//{
			//	writer.WriteLine($"time\t{string.Join("\t", fieldsNames)}");
			//	foreach (var item in fieldRawValue)
			//	{
			//		writer.Write($"{item.Time}\t");
			//		foreach (var fieldsName in fieldsNames)
			//		{
			//			double value;
			//			if (item.Values.TryGetValue(fieldsName, out value))
			//				writer.Write(value);
			//			writer.Write("\t");
			//		}
			//		writer.WriteLine();
			//	}
			//}

			var fieldValue = new FieldSpecificExtractor().Extract(result).ToList();

			//var trips = new TripExtractor().Extract(fieldValue);

			//IFirebaseConfig config = new FirebaseConfig
			//{
			//	AuthSecret = "kpI2kVDB3WqSjSClhoK9XftXCpuuTvbu5BkEJPBA",
			//	BasePath = "https://hackzurich-1ced0.firebaseio.com/"
			//};
			//IFirebaseClient client = new FirebaseClient(config);

			//for (int i = 0; i < trips.Count; i++)
			//{
			//	var trip = trips[i];

			//	if (trip.Distance < 1)
			//		continue;

			//	var path = Path.Combine(fieldPath, $"{new FileInfo(file).Name}_trip_{i}");
			//	WriteRecords(path, trip.Records);

			//	Console.WriteLine($"Trip {i}: Distance: {trip.Distance:0.00}km, Fuel: {trip.Fuel}ml");

			//	var coordinates = Deduplicate(trip.Records.Where(a => a.Position != null).Select(a => a.Position));
			//	var collection = new FeatureCollection();
			//	collection.Features.Add(new Feature(new LineString(coordinates)));

			//	var geopath = Path.Combine(fieldPath, $"{deviceId}_trip_{i}_geo");
			//	using (var stream = new StreamWriter(new FileStream(geopath, FileMode.Create, FileAccess.Write)))
			//	{
			//		_serializer.Serialize(stream, collection);
			//	}

			//	var tripData = new TripData
			//	{
			//		DeviceId = deviceId,
			//		Distance = trip.Distance,
			//		Start = trip.Start,
			//		End = trip.End,
			//		Fuel = trip.Fuel,
			//		Route = trip.Records.Where(a => a.Position != null).Select(a => new[] { a.Position.Latitude, a.Position.Longitude })
			//	};
			//	//await client.SetAsync($"schaelle/trips/{new FileInfo(file).Name}/{i}", tripData);
			//}
			////fieldValue = MovingAverage(fieldValue);

			return fieldValue;
		}

		private static void WriteRecords<TData>(string file, IEnumerable<TData> items)
		{
			using (var writer = new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write)))
			{
				var csv = new CsvWriter(writer, new CsvConfiguration { Delimiter = "\t" });
				csv.WriteHeader<TData>();
				foreach (var item in items)
				{
					csv.WriteRecord(item);
				}
			}
		}

		private static List<Tuple<DateTime, double[]>> MovingAverage(List<Tuple<DateTime, double[]>> fieldValue)
		{
			var res = new List<Tuple<DateTime, double[]>>();

			var interval = TimeSpan.FromSeconds(60);

			var latest = DateTime.MinValue;
			var set = false;
			var count = 0;
			var values = new double[0];

			foreach (var tuple in fieldValue)
			{
				var bucket = Bucket(tuple.Item1, interval);
				if (!set || bucket != latest)
				{
					if (set)
					{
						for (var i = 0; i < tuple.Item2.Length; i++)
						{
							values[i] /= count;
						}
						res.Add(new Tuple<DateTime, double[]>(latest, values));
					}

					latest = bucket;
					values = new double[tuple.Item2.Length];
					for (var i = 0; i < tuple.Item2.Length; i++)
					{
						values[i] = tuple.Item2[i];
					}
					count = 0;
				}
				else
				{
					for (var i = 0; i < tuple.Item2.Length; i++)
					{
						values[i] += tuple.Item2[i];
					}
					count++;
				}
				set = true;
			}

			return res;
		}

		private static DateTime Bucket(DateTime time, TimeSpan interval)
		{
			return DateTime.MinValue.AddSeconds((int)((time - DateTime.MinValue).TotalSeconds / interval.TotalSeconds) * interval.TotalSeconds);
		}

		private static IEnumerable<GeographicPosition> Deduplicate(IEnumerable<GeographicPosition> positions)
		{
			GeographicPosition previous = null;
			foreach (var position in positions)
			{
				double TOLERANCE = 1E-12;
				if (previous == null || Math.Abs(previous.Latitude - position.Latitude) > TOLERANCE || Math.Abs(previous.Longitude - position.Longitude) > TOLERANCE)
				{
					previous = position;
					yield return position;
				}
			}
		}

		private class TripData
		{
			public string DeviceId { get; set; }
			public double Distance { get; set; }
			public DateTime Start { get; set; }
			public DateTime End { get; set; }
			public double Fuel { get; set; }
			public IEnumerable<double[]> Route { get; set; }
		}
	}
}