﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Serialization;

namespace SuiBotAI.Components.Other.Gemini
{
	[Serializable]
	public class GeminiMessage
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public Role role;
		public GeminiResponseMessagePart[] parts;

		public static GeminiMessage CreateMessage(string contentToAsk, Role role)
		{
			return new GeminiMessage()
			{
				role = role,
				parts = new GeminiResponseMessagePart[]
				{
					new GeminiResponseMessagePart()
					{
						text = contentToAsk.Trim()
					}
				}
			};
		}

		public static GeminiMessage CreateFunctionCallResponse(string functionName, string functionResponse)
		{
			return new GeminiMessage()
			{
				role = Role.user,
				parts = new GeminiResponseMessagePart[]
				{
					new GeminiResponseMessagePart()
					{
						functionResponse = new GeminiFunctionResponse()
						{
							name = functionName,
							response = new GeminiFunctionResponse.GeminiFunctionResponseRawTex
							{
								content = functionResponse
							}
						}
					}
				}
			};
		}

		public static GeminiMessage CreateInlineData(string mimeType, byte[] bytes)
		{
			if (bytes.Length > 20_000_000)
				throw new Exception("Too much data to send!");

			return new GeminiMessage()
			{
				role = Role.user,
				parts = new GeminiResponseMessagePart[]
				{
					new GeminiResponseMessagePart()
					{
						inlineData = new GeminiResponseInlineData()
						{
							mimeType =mimeType,
							data = Convert.ToBase64String(bytes)
						}
					}
				}
			};
		}
	}



	[Serializable]
	public class GeminiResponseMessagePart
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string text = null;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public GeminiResponseFunctionCall functionCall = null;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public GeminiFunctionResponse functionResponse = null;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public GeminiResponseInlineData inlineData = null;
	}

	[Serializable]
	public class GeminiResponseFunctionCall
	{
		public string name = "";
		[XmlIgnore] public JToken args = null;
	}

	[Serializable]
	public class GeminiFunctionResponse
	{
		public class GeminiFunctionResponseRawTex
		{
			public string content = "";
		}

		public string name = "";
		public GeminiFunctionResponseRawTex response = null;
	}

	[Serializable]
	public class GeminiResponseInlineData
	{
		public string mimeType = "";
		public string data = "";
	}

	public enum Role
	{
		user,
		model,
		summery
	}
}
