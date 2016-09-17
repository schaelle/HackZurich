using System.Collections.Generic;

namespace HackZurich.Parser
{
	public interface IExtractor<TInput, TResult>
	{
		IReadOnlyList<TResult> Extract(IEnumerable<TInput> records);
	}
}