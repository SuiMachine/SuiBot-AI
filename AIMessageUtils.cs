using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuiBotAI.Components;
using SuiBotAI.Components.Other.Gemini;
using System;
using System.Collections.Generic;
using System.IO;

namespace SuiBotAI
{
	public static class AIMessageUtils
	{
		public static string AppendDateTimePrefix(string message)
		{
			System.Globalization.CultureInfo globalizationOverride = new System.Globalization.CultureInfo("en-US");

			return $"[DATETIME: Local {DateTime.Now.ToString("yyy-MM-dd", globalizationOverride)} {DateTime.Now:HHH:mm:ss} | UTC {DateTime.UtcNow.ToString("yyy-MM-dd", globalizationOverride)} {DateTime.UtcNow:HH:mm:ss}Z]: {message}";
		}

		public static List<GeminiMessage> ImportFromGoogleFile(string file)
		{
			var convertedMessages = new List<GeminiMessage>();
			var content = (JToken)JsonConvert.DeserializeObject(File.ReadAllText(file));
			if (content["chunkedPrompt"]?["chunks"] == null)
				throw new Exception("Invalid file");
			var chunks = content["chunkedPrompt"]["chunks"];
			ulong tokenCount = 0;
			foreach (var chunk in chunks)
			{
				var role = chunk["role"].ToObject<Role>();
				var messageTokens = chunk["tokenCount"].Value<ulong>();
				tokenCount += messageTokens;

				if (chunk["text"] != null)
				{
					var messageContent = chunk["text"].Value<string>();
					convertedMessages.Add(GeminiMessage.CreateMessage(messageContent, role));
				}
				else if (chunk["resolvedFunctionCall"] != null)
				{
					var callName = chunk["resolvedFunctionCall"]?["functionCall"]?["name"];
					if (callName == null)
						continue;
					convertedMessages.Add(new GeminiMessage()
					{
						role = role,
						parts = new GeminiResponseMessagePart[]
						{
							new GeminiResponseMessagePart()
							{
								functionCall = new GeminiResponseFunctionCall()
								{
									name = callName.Value<string>()
								}
							}
						}
					});
				}
			}
			return convertedMessages;
		}

		public static void SummarizeMessages(SuiBotAIProcessor processor, GeminiContent content)
		{

		}
	}
}
