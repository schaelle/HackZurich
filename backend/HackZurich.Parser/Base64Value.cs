using Newtonsoft.Json;

namespace HackZurich.Parser
{
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