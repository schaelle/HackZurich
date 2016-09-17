using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HackZurich.Parser
{
	class Program
	{
		static void Main(string[] args)
		{
			AsyncMain().Wait();
		}

		public static async Task AsyncMain()
		{
			var parser = new FileParser();

			const string path = @"D:\Projects\HackZurich\backend\rawData\temp\json";

			var tasks = new List<Task>();
			foreach (var file in Directory.GetFiles(path))
			{
				tasks.Add(parser.ParseAsync(file));
			}

			await Task.WhenAll(tasks);
			Console.WriteLine("Completed");
			Console.ReadLine();
		}
	}

	public class FileParser
	{
		private readonly JsonSerializer _serializer = new JsonSerializer();

		public async Task<IReadOnlyCollection<Record>> ParseAsync(string file)
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

			Console.WriteLine($"{file} #{result.Count}");

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
			fieldsNames = new[] { "GPS_SPEED", "ODO_FULL_METER", "MDI_OBD_RPM" };
			var fieldValue = new FieldExtractor(fieldsNames).Extract(result).ToList();
			fieldValue = MovingAverage(fieldValue);

			using (var stream = new StreamWriter(new FileStream(Path.Combine(fieldPath, new FileInfo(file).Name), FileMode.Create, FileAccess.Write)))
			{
				stream.Write("\t");
				stream.WriteLine(string.Join("\t", fieldsNames));
				foreach (var value in fieldValue)
				{
					stream.Write(value.Item1 + "\t");
					stream.WriteLine(string.Join("\t", value.Item2));
				}
			}

			return result;
		}

		private List<Tuple<DateTime, double[]>> MovingAverage(List<Tuple<DateTime, double[]>> fieldValue)
		{
			var res = new List<Tuple<DateTime, double[]>>();

			var interval = TimeSpan.FromSeconds(60);

			var latest = DateTime.MinValue;
			var set = false;
			var count = 0;
			double[] values = new double[0];

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
	}

	public interface IExtractor<TResult>
	{
		IEnumerable<TResult> Extract(IEnumerable<Record> records);
	}

	public class PathExtractor : IExtractor<Tuple<DateTime, double[]>>
	{
		public IEnumerable<Tuple<DateTime, double[]>> Extract(IEnumerable<Record> records)
		{
			foreach (var record in records)
			{
				if (record.Location != null)
				{
					yield return new Tuple<DateTime, double[]>(record.RecorderdAt, record.Location);
				}
			}
		}
	}

	public class FieldExtractor : IExtractor<Tuple<DateTime, double[]>>
	{
		private readonly string[] _fields;

		public FieldExtractor(string[] fields)
		{
			_fields = fields;
		}

		public IEnumerable<Tuple<DateTime, double[]>> Extract(IEnumerable<Record> records)
		{
			foreach (var record in records)
			{
				var data = new double[_fields.Length];
				for (var i = 0; i < _fields.Length; i++)
				{
					var field = _fields[i];

					Base64Value value;
					if (record.Fields.TryGetValue(field, out value))
					{
						data[i] = value.AsInt();
					}
				}

				yield return new Tuple<DateTime, double[]>(record.RecorderdAt, data);
			}
		}
	}

	public class FieldNameExtractor : IExtractor<string>
	{
		public IEnumerable<string> Extract(IEnumerable<Record> records)
		{
			var fields = new HashSet<string>();
			foreach (var record in records)
			{
				foreach (var key in record.Fields.Keys)
				{
					fields.Add(key);
				}
			}
			return fields;
		}
	}

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
	}

	public class Base64Value
	{
		[JsonProperty("b64_value")]
		public string Value { get; set; }

		public int AsInt()
		{
			return Base64Decoder.GetValueAsInt(Value);
		}
	}
}
