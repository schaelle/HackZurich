using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GeoJSON.Net.Feature;
using Newtonsoft.Json.Linq;

namespace HackZurich.Parser
{
	class Program
	{
		private const string Id = "356156068026087_porscheinte_2016-09-04T14_53_43Z_2016-09-05T15_17_41Z";

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

			//await parser.ParseAsync(Path.Combine(path, Id));
			Console.WriteLine("Completed");
			Console.ReadLine();
		}
	}
}
