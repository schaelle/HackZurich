using System;

namespace HackZurich.Parser
{
	public static class Base64Decoder
	{
		public static bool GetValueAsBool(string b64Value)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(b64Value))
			{
				byte[] values = Convert.FromBase64String(b64Value);
				result = BitConverter.ToBoolean(values, 0);
			}
			return result;
		}

		public static int GetValueAsInt(string b64Value)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(b64Value))
			{
				byte[] values = Convert.FromBase64String(b64Value);

				if (values.Length > 0)
				{
					result = 0;
					for (int i = 0; i < values.Length; i++)
					{
						result = result << 8;
						result += values[i];
					}
				}
			}
			return result;
		}

		public static string GetValueAsString(string b64_value)
		{
			string result = String.Empty;
			if (!String.IsNullOrEmpty(b64_value))
			{
				byte[] values = Convert.FromBase64String(b64_value);
				result = String.Empty;
				for (int i = 0; i < values.Length; i++)
				{
					result += (char)values[i];
				}
			}
			return result;
		}
	}
}