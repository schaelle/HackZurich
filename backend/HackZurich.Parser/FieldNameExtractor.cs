using System.Collections.Generic;
using System.Linq;

namespace HackZurich.Parser
{
	public class FieldNameExtractor : IExtractor<Record, string>
	{
		public IReadOnlyList<string> Extract(IEnumerable<Record> records)
		{
			var fields = new HashSet<string>();
			foreach (var record in records)
			{
				foreach (var key in record.Fields.Keys)
				{
					fields.Add(key);
				}
			}
			return fields.ToList();
		}
	}
}