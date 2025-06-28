using System;

namespace SuiBotAI
{
	public static class AIMessageUtils
	{
		public static string AppendDateTimePrefix(string message)
		{
			System.Globalization.CultureInfo globalizationOverride = new System.Globalization.CultureInfo("en-US");

			return $"[DATETIME: Local {DateTime.Now.ToString("yyy-MM-dd", globalizationOverride)} {DateTime.Now:HHH:mm:ss} | UTC {DateTime.UtcNow.ToString("yyy-MM-dd", globalizationOverride)} {DateTime.UtcNow:HH:mm:ss}Z]: {message}";
		}
	}
}
