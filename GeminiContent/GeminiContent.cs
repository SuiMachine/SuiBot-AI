using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SuiBotAI.Components.Other.Gemini
{
	[Serializable]
	public class GeminiContent
	{
		[Serializable]
		public class GenerationConfig
		{
			[JsonIgnore] public int TokenCount = 0;
			public float temperature = 1f;
			[XmlIgnore] public float topK = 20;
			[XmlIgnore] public float topP = 0.95f;
			[XmlIgnore] public int maxOutputTokens = 8192;
			[XmlIgnore] public string responseMimeType = "text/plain";
		}

		public List<GeminiMessage> contents;
		[XmlIgnore][JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public GeminiSafetySettingsCategory[] safetySettings;

		[XmlIgnore]	public GeminiMessage systemInstruction;

		[XmlIgnore]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<GeminiTools> tools;

		public GenerationConfig generationConfig;
	}

	public class GeminiSafetySettingsCategory
	{
		public string category;
		[JsonConverter(typeof(StringEnumConverter))]
		public AISafetySettingsValues threshold;
		[XmlIgnore]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<GeminiTools> tools;

		public static List<GeminiTools> GetTools()
		{
			return new List<GeminiTools>()
			{
				new GeminiTools()
			};
		}

		public enum AISafetySettingsValues
		{
			BLOCK_NONE,
			BLOCK_ONLY_HIGH, //Block few
			BLOCK_MEDIUM_AND_ABOVE, //Block some
			BLOCK_LOW_AND_ABOVE //Block Most
		}

		public GeminiSafetySettingsCategory(string category, AISafetySettingsValues safety)
		{
			this.category = category;
			this.threshold = safety;
		}
	}


	[Serializable]
	public class GeminiResponse
	{
		[Serializable]
		public class GeminiResponseCandidates
		{
			public GeminiMessage content;
			public string finishReason = "";
			public double avgLogprobs = 0;
		}

		[Serializable]
		public class UsageMetadata
		{
			public int promptTokenCount = 0;
			public int candidatesTokenCount = 0;
			public int totalTokenCount = 0;
		}

		public GeminiResponseCandidates[] candidates;
		public UsageMetadata usageMetadata;
		public string modelVersion;
	}
}
