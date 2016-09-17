using System.Collections.Generic;
using System.Linq;

namespace HackZurich.Parser
{
	public class PointProducer
	{
		public UserPoints Compute(TripRecord trip, IReadOnlyList<AlternateTripProducer.TripDetail> alternative)
		{
			var dict = alternative.Where(i => i != null).ToDictionary(i => i.Mode);

			var trafficKey = "driving";
			var transitKey = "transit";
			double? ratio = null;
			if (dict.ContainsKey(trafficKey) && dict.ContainsKey(transitKey))
			{
				ratio = (dict[transitKey].Co2 / dict[trafficKey].Co2) /
						(dict[trafficKey].Duration/ dict[transitKey].Duration);
			}
			return new UserPoints
			{
				CarVsTransit = 100 * ratio
			};
		}
	}
}