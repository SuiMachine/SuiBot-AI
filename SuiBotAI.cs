using Newtonsoft.Json;
using SuiBot_TwitchSocket;
using SuiBotAI.Components.Other.Gemini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuiBotAI.Components
{
	public class SuiBotAIProcessor
	{
		public class FailedToGetResponseException : Exception
		{
			public FailedToGetResponseException() { }
			public FailedToGetResponseException(string message) : base(message)
			{
				Private = message;
			}

			public string PublicMessage;
			public string Private;
		}

		public class SafetyFilterTrippedException : Exception
		{
			public SafetyFilterTrippedException() { }
			public SafetyFilterTrippedException(string message) : base(message)
			{
				SafetyTriped = message;
			}

			public string SafetyTriped;
		}

		private readonly string m_API_Key;
		private readonly string m_Model;

		public SuiBotAIProcessor(string API_Key, string Model)
		{
			m_API_Key = API_Key;
			m_Model = Model;
		}

		public async Task<GeminiResponse> GetAIResponse(GeminiContent content, GeminiMessage systemInstruction, GeminiMessage messageToAppend)
		{
			try
			{
				content.systemInstruction = systemInstruction;

				if (content == null)
					throw new ArgumentNullException("Content was null!");


				content.contents.Add(messageToAppend);

				string json = JsonConvert.SerializeObject(content, Formatting.Indented);

				string result = await HttpWebRequestHandlers.PerformPostAsync("https://generativelanguage.googleapis.com/", $"v1beta/{m_Model}:generateContent", $"?key={m_API_Key}",
					json,
					new Dictionary<string, string>(), timeout: 25_000
				);

				if (string.IsNullOrEmpty(result))
					throw new ArgumentNullException("Failed to get a response. Please debug me, Sui :(");
				else
				{
					GeminiResponse response = JsonConvert.DeserializeObject<GeminiResponse>(result);
					if (response == null)
						throw new NullReferenceException("Failed to deserialize response");

					var stopReason = response.candidates.Last().finishReason;
					if (stopReason == "SAFETY")
						throw new SafetyFilterTrippedException();
					else if (stopReason == "MAX_TOKENS")
						throw new FailedToGetResponseException("Reached max token limit!");

					return response;
				}
			}
			catch (Exception ex)
			{
				throw new FailedToGetResponseException()
				{
					PublicMessage = "Failed to get a response. Something was written in log. Sui help! :(",
					Private = ex.ToString()
				};
			}
		}

		public static void CleanupResponse(ref string text)
		{
			List<string> splitText = text.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries).ToList();
			for (int i = splitText.Count - 1; i >= 0; i--)
			{
				var line = splitText[i].Trim();
				if (line.StartsWith("*") && line.StartsWith("*"))
				{
					var count = line.Count(x => x == '*');
					if (count == 2)
					{
						splitText.RemoveAt(i);
						continue;
					}
				}

				if (line.Contains("*"))
				{
					line = CleanDescriptors(line);
					splitText[i] = line;
				}
			}

			text = string.Join(" ", splitText);
		}

		private static string CleanDescriptors(string text)
		{
			int endIndex = text.Length;
			bool isDescription = false;

			for (int i = text.Length - 1; i >= 0; i--)
			{
				if (text[i] == '*')
				{
					if (!isDescription)
					{
						endIndex = i;
						isDescription = true;
					}
					else
					{
						var length = i - endIndex;
						var substring = text.Substring(i + 1, endIndex - i - 1);
						if (substring.Split([' '], StringSplitOptions.RemoveEmptyEntries).Length > 5)
						{
							text = text.Remove(i, endIndex - i + 1);
						}
						isDescription = false;
					}
				}
			}

			while (text.Contains("  "))
			{
				text = text.Replace("  ", " ");
			}

			text = text.Trim();
			return text;
		}
	}
}
